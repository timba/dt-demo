using System.IO;
using System.Linq;

namespace DTDemo.DealProcessing
{
    public class DealStatService : IDealStatService
    {
        public (string, int)? GetMostOftenSoldVehicle(DealRecord[] records)
        {
            if (records.Length == 0)
            {
                return null;
            }

            var groupped = from record in records
                           group record by record.Vehicle into @group
                           select new { name = @group.Key, count = @group.Count() };

            var max = groupped.OrderByDescending(group => group.count).First();

            return (max.name, max.count);
        }
    }
}