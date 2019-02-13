using System.IO;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing
{
    public interface IDealRecordService
    {
        Task<DealRecord[]> GetDeals(TextReader reader);
    }
}