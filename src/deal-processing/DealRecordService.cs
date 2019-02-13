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
            string[][] records = null;
            
            try
            {
                records = await this.fileParser.Parse(reader);
            }
            catch (ParseException ex)
            {
                throw new InvalidDealRecordFileException($"CSV has error at line {ex.Line} position {ex.Column}: {ex.Message}");
            }

            try
            {
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
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is FormatException)
            {
                throw new InvalidDealRecordFileException("The file is not valid Deals Records table or contains invalid data");
            }
        }
    }
}
