using System.IO;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing.Csv
{
    public interface IFileParser
    {
        Task<string[][]> Parse(TextReader reader);
    }
}