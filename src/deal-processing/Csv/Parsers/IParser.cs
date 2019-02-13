namespace DTDemo.DealProcessing.Csv
{
    public interface IParser
    {
        (char?, ParserType) Parse(char character);
        
        ParserType ParserType { get; }
    }
}