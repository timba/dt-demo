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

            int brokenLine = 0;
            try
            {
                return records.Select((it, line) =>
                {
                    brokenLine = line;
                    return new DealRecord
                    {
                        Id = Int32.Parse(it[0]),
                        CustomerName = it[1],
                        DealershipName = it[2],
                        Vehicle = it[3],
                        Price = Single.Parse(it[4]),
                        Date = it[5]
                    };
                }
                ).ToArray();
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidDealRecordFileException(
                    $"The file is not valid Deals Records table. It contains invalid columns count at data record {brokenLine}.");
            }
            catch (FormatException ex)
            {
                throw new InvalidDealRecordFileException(
                    $"The file is not valid Deals Records table. It contains invalid format value at data record {brokenLine}. Error: {ex.Message}");
            }
        }
    }
}
