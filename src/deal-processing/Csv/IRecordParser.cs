using System.IO;

namespace DTDemo.DealProcessing.Csv
{
    public interface IRecordParser
    {
        string[] Parse(string input);
    }
}