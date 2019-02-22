namespace DTDemo.DealProcessing.Csv
{
    public class GenericParser : IParser
    {
        private readonly char delimeter;

        public GenericParser(char delimeter)
        {
            this.delimeter = delimeter;
        }

        public ParserType ParserType => ParserType.Generic;

        public (char?, ParserType) Parse(char character)
        {
            switch (character)
            {
                case char c when c == this.delimeter:
                    return (null, ParserType.Initial);
                case Symbol.Quote:
                    throw new ParseException("Quote symbol not expected in unquoted field");
                case Symbol.Newline:
                    return (null, ParserType.NewLine);
                default:
                    return (character, ParserType.Generic);
            }
        }
    }
}