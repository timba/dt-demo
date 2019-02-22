namespace DTDemo.DealProcessing.Csv
{
    public class QuoteParser : IParser
    {
        private readonly char delimeter;

        public QuoteParser(char delimeter)
        {
            this.delimeter = delimeter;
        }

        public ParserType ParserType => ParserType.Quote;

        public (char?, ParserType) Parse(char character)
        {
            switch (character)
            {
                case char c when c == this.delimeter:
                    return (null, ParserType.Initial);
                case Symbol.Quote:
                    return (Symbol.Quote, ParserType.String);
                case Symbol.Newline:
                    return (null, ParserType.NewLine);
                default:
                    throw new ParseException(
                        $"After quote, expected another quote (\") or delimeter ({this.delimeter}), but '{character}' found");
            }
        }
    }
}