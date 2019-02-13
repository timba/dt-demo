namespace DTDemo.DealProcessing.Csv
{
    public class InitialParser : IParser
    {
        private readonly char delimeter;

        public InitialParser(char delimeter)
        {
            this.delimeter = delimeter;
        }

        public ParserType ParserType => ParserType.Initial;

        public (char?, ParserType) Parse(char character)
        {
            switch (character)
            {
                case char c when c == this.delimeter:
                    return (null, ParserType.Initial);
                case Symbol.Quote:
                    return (null, ParserType.String);
                default:
                    return (character, ParserType.Generic);
            }
        }
    }
}