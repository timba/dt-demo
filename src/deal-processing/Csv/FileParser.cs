using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing.Csv
{
    public class FileParser : IFileParser
    {
        private readonly IRecordParser recordParser;
        
        private readonly bool skipHeader;

        public FileParser(IRecordParser recordParser, bool skipHeader)
        {
            this.recordParser = recordParser;
            this.skipHeader = skipHeader;
        }

        public async Task<string[][]> Parse(TextReader reader)
        {
            var table = new List<string[]>();
            var record = reader.ReadLine();
            int line = 0;
            if (this.skipHeader)
            {
                record = await reader.ReadLineAsync();
                line++;
            }

            while (record != null)
            {
                try
                {
                    table.Add(this.recordParser.Parse(record));
                }
                catch (ParseException ex)
                {
                    ex.Line = line;
                    throw ex;
                }

                record = await reader.ReadLineAsync();
                line++;
            }

            return table.ToArray();
        }
    }
}