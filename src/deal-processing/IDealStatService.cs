using System.IO;

namespace DTDemo.DealProcessing
{
    public interface IDealStatService
    {
        (string, int)? GetMostOftenSoldVehicle(DealRecord[] records);
    }
}