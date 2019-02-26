using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

using DTDemo.DealProcessing;
using DTDemo.Server.Hubs;

namespace DTDemo.Server.Controllers
{
    [Route("api/deals")]
    public class DealsDataController : Controller
    {
        private readonly IDealRecordService dealRecordService;
        private readonly IDealRecordStatAccumulator dealRecordStatObserver;
        private readonly Encoding csvEncoding;
        private readonly IHubContext<DealsHub, IDealsHub> hubContext;

        public DealsDataController(IDealRecordService dealRecordService, IDealRecordStatAccumulator dealRecordStatObserver, Encoding csvEncoding, IHubContext<DealsHub, IDealsHub> hubContext)
        {
            this.dealRecordService = dealRecordService;
            this.dealRecordStatObserver = dealRecordStatObserver;
            this.csvEncoding = csvEncoding;
            this.hubContext = hubContext;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            var contentType = this.Request.ContentType;
            if (!contentType.StartsWith("multipart/form-data"))
            {
                BadRequest("Expected multipart form data");
            }

            var mediaType = MediaTypeHeaderValue.Parse(this.Request.ContentType);
            var boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary);
            var mpReader = new MultipartReader(boundary.Value, this.Request.Body);
            var section = await mpReader.ReadNextSectionAsync();
            var connectionId = await section.ReadAsStringAsync();
            section = await mpReader.ReadNextSectionAsync();

            using (var stream = section.Body)
            using (StreamReader reader = new StreamReader(stream, this.csvEncoding))
            {
                Console.WriteLine($"Connection ID: '{connectionId}'");
                var client = this.hubContext.Clients.Client(connectionId);
                await client.start();

                using (var evt = new SemaphoreSlim(0))
                using (
                    this.dealRecordService
                        .GetDeals(reader)
                        .Do(this.dealRecordStatObserver.Scan)
                        .Subscribe(
                            // New deals record parsed
                            async record =>
                            {
                                await client.d(new DealRecordView
                                {
                                    Id = record.Id,
                                    CustomerName = record.CustomerName,
                                    DealershipName = record.DealershipName,
                                    Vehicle = record.Vehicle,
                                    Price = $"CAD${record.Price:#,#.00}",
                                    Date = record.Date
                                });
                            },

                            // Error occurred
                            async error =>
                            {
                                await client.error(error.Message);
                                Console.WriteLine($"Error pushed: {error}");
                                evt.Release();
                            },

                            // Stream processing completed
                            async () =>
                            {
                                var stat = this.dealRecordStatObserver.GetMostOftenSoldVehicle();
                                if (stat.HasValue)
                                {
                                    var (name, count) = stat.Value;
                                    await client.stat(new MostSoldVehicleView { Name = name, Count = count });
                                }
                                else
                                {
                                    await client.stat(null);
                                }

                                Console.WriteLine($"Stat pushed");
                                evt.Release();
                            }
                        )
                )
                {
                    await evt.WaitAsync();
                }
            }

            return NoContent();
        }

        public class MostSoldVehicleView
        {
            public string Name { get; set; }

            public int Count { get; set; }
        }

        public class DealRecordsSummaryView
        {
            public DealRecordView[] Deals { get; set; }

            public MostSoldVehicleView MostSoldVehicle { get; set; }
        }

        public class DealRecordView
        {
            [JsonProperty("i")]
            public int Id { get; set; }

            [JsonProperty("n")]
            public string CustomerName { get; set; }

            [JsonProperty("d")]
            public string DealershipName { get; set; }

            [JsonProperty("v")]
            public string Vehicle { get; set; }

            [JsonProperty("p")]
            public string Price { get; set; }

            [JsonProperty("t")]
            public string Date { get; set; }
        }
    }
}
