namespace DTDemo.DealProcessing.Csv
{
    public class NewlineParser : IParser
    {
        private readonly char delimeter;

        public NewlineParser(char delimeter)
        {
            this.delimeter = delimeter;
        }

        public ParserType ParserType => ParserType.NewLine;

        public (char?, ParserType) Parse(char character)
        {
            switch (character)
            {
                case char c when c == this.delimeter:
                    return (null, ParserType.Initial);
                case Symbol.Quote:
                    return (null, ParserType.String);
                case Symbol.Newline:
                    return (null, ParserType.NewLine);
                default:
                    return (character, ParserType.Generic);
            }
        }
    }
}