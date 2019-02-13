using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing.Csv
{
    public class RecordParser : IRecordParser
    {
        private readonly Dictionary<ParserType, IParser> parsers;

        public RecordParser(IParser[] parsers)
        {
            this.parsers = parsers.ToDictionary(kv => kv.ParserType);
        }

        public string[] Parse(string input)
        {
            var fields = new List<string>();
            var parser = this.parsers[ParserType.Initial];
            var field = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char symbol = input[i];
                try
                {
                    var (parsed, nextParser) = parser.Parse(symbol);

                    if (parsed.HasValue)
                    {
                        field.Append(parsed.Value);
                    }

                    if ((nextParser == ParserType.Initial && parser.ParserType != ParserType.Initial)
                    || nextParser == ParserType.NewLine)
                    {
                        fields.Add(field.ToString().Trim());
                        field = new StringBuilder();
                    }

                    parser = parsers[nextParser];
                }
                catch (ParseException ex)
                {
                    ex.Column = i;
                    throw ex;
                }
            }

            if (parser.ParserType == ParserType.String)
            {
                throw new ParseException($"Quoted record has not been closed but the record ended")
                { Column = input.Length - 1 };
            }

            fields.Add(field.ToString().Trim());
            return fields.ToArray();
        }
    }
}