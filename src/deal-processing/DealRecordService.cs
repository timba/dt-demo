using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using DTDemo.DealProcessing.Csv;

namespace DTDemo.DealProcessing
{
    public class DealRecordService : IDealRecordService
    {
        private readonly IRecordParser parser;
        private readonly bool skipHeader;

        public DealRecordService(IRecordParser parser, bool skipHeader)
        {
            this.parser = parser;
            this.skipHeader = skipHeader;
        }

        public IObservable<DealRecord> GetDeals(TextReader reader)
        {
            return this.parser.Parse(reader)
            .Catch<(string[], int), ParseException>(ex =>
                 Observable.Throw<(string[], int)>(new InvalidDealRecordFileException($"CSV has error at line {ex.Line} position {ex.Column}: {ex.Message}")))
            .Skip(this.skipHeader ? 1 : 0)
            .Where(record => 
                record.Item1.Length > 1 || 
                (record.Item1.Length == 1 && record.Item1[0].Length > 0))
            .Select(record =>
            {
                var (cols, line) = record;
                try
                {
                    return new DealRecord
                    {
                        Id = Int32.Parse(cols[0]),
                        CustomerName = cols[1],
                        DealershipName = cols[2],
                        Vehicle = cols[3],
                        Price = Single.Parse(cols[4]),
                        Date = cols[5]
                    };
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidDealRecordFileException(
                        $"The file is not valid Deals Records table. It contains invalid columns count at data record {line}.");
                }
                catch (FormatException ex)
                {
                    throw new InvalidDealRecordFileException(
                        $"The file is not valid Deals Records table. It contains invalid format value at data record {line}. Line: {string.Join(",", cols)}. Error: {ex.Message}");
                }
            });
        }
    }
}
