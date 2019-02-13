using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DTDemo.DealProcessing.Csv;

namespace DTDemo.DealProcessing
{
    public class DealRecordService : IDealRecordService
    {
        private readonly IFileParser fileParser;

        public DealRecordService(IFileParser fileParser)
        {
            this.fileParser = fileParser;
        }

        public async Task<DealRecord[]> GetDeals(TextReader reader)
        {
            var records = await this.fileParser.Parse(reader);
            return records.Select(it =>
                new DealRecord
                {
                    Id = Int32.Parse(it[0]),
                    CustomerName = it[1],
                    DealershipName = it[2],
                    Vehicle = it[3],
                    Price = Single.Parse(it[4]),
                    Date = DateTime.Parse(it[5])
                }
            ).ToArray();
        }
    }
}
