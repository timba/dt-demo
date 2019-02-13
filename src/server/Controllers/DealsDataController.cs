using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DTDemo.DealProcessing;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DTDemo.Server.Controllers
{
    [Route("api/deals")]
    public class DealsDataController : Controller
    {
        private readonly IDealRecordService dealRecordService;
        private readonly IDealStatService statService;
        private readonly Encoding csvEncoding;

        public DealsDataController(IDealRecordService dealRecordService, IDealStatService statService, Encoding csvEncoding)
        {
            this.dealRecordService = dealRecordService;
            this.statService = statService;
            this.csvEncoding = csvEncoding;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var stream = file.OpenReadStream();
            StreamReader reader = new StreamReader(stream, csvEncoding);
            DealRecord[] deals;
            try
            {
                deals = await this.dealRecordService.GetDeals(reader);
            }
            catch (InvalidDealRecordFileException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }

            var mostOftenSold = this.statService.GetMostOftenSoldVehicle(deals);

            var dealsView = deals.Select(it =>
                new DealRecordView
                {
                    Id = it.Id,
                    CustomerName = it.CustomerName,
                    DealershipName = it.DealershipName,
                    Vehicle = it.Vehicle,
                    Price = $"CAD${it.Price:#,#.00}",
                    Date = it.Date.ToShortDateString()
                }).ToArray();

            return Ok(new DealRecordsSummaryView
            {
                Deals = dealsView,
                MostSoldVehicle = mostOftenSold.HasValue ? new MostSoldVehicleView
                {
                    Name = mostOftenSold.Value.Item1,
                    Count = mostOftenSold.Value.Item2
                } : null
            });
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
            public int Id { get; set; }
            public string CustomerName { get; set; }
            public string DealershipName { get; set; }
            public string Vehicle { get; set; }
            public string Price { get; set; }
            public string Date { get; set; }
        }
    }
}
