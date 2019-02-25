using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace DTDemo.DealProcessing.Csv
{
    public class RecordParser : IRecordParser
    {
        private readonly Dictionary<ParserType, IParser> parsers;

        public RecordParser(IParser[] parsers)
        {
            this.parsers = parsers.ToDictionary(kv => kv.ParserType);
        }

        public IObservable<(string[], int)> Parse(TextReader file)
        {
            return Observable.Create<(string[], int)>(async observer =>
            {
                var buffer = new char[128];
                var position = 0;
                var read = 0;

                var fields = new List<string>();
                var parser = this.parsers[ParserType.Initial];
                var field = new LinkedList<char>();

                int line = 0, col = 0;

                while ((read = await file.ReadBlockAsync(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        char symbol = buffer[i];
                        try
                        {
                            var (parsed, nextParser) = parser.Parse(symbol);

                            if (parsed.HasValue)
                            {
                                field.AddLast(parsed.Value);
                            }

                            if (nextParser == ParserType.Initial || nextParser == ParserType.NewLine)
                            {
                                fields.Add(new string(field.Trim().ToArray()));
                                field = new LinkedList<char>();
                            }

                            if (nextParser == ParserType.NewLine)
                            {
                                observer.OnNext((fields.ToArray(), line));
                                fields.Clear();
                                line++;
                                col = 0;
                            }

                            parser = parsers[nextParser];
                        }
                        catch (ParseException ex)
                        {
                            ex.Column = col;
                            ex.Line = line;
                            observer.OnError(ex);
                            return;
                        }

                        col++;
                    }

                    position = position + read;
                }

                if (parser.ParserType == ParserType.String)
                {
                    observer.OnError(new ParseException($"Quoted record has not been closed but the file ended")
                    {
                        Column = col,
                        Line = line
                    });

                    return;
                }

                fields.Add(new string(field.Trim().ToArray()));
                observer.OnNext((fields.ToArray(), line));
                observer.OnCompleted();

            });
        }
    }
}