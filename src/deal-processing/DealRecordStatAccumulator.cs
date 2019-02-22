using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing
{
    public class DealRecordStatAccumulator : IDealRecordStatAccumulator
    {
        private readonly Dictionary<string, int> stat = new Dictionary<string, int>();

        public (string, int)? GetMostOftenSoldVehicle()
        {
            if (this.stat.Count == 0)
            {
                return null;
            }

            var top = this.stat.OrderByDescending(kv => kv.Value).First();
            return (top.Key, top.Value);
        }

        public void Scan(DealRecord record)
        {
            if (this.stat.TryGetValue(record.Vehicle, out int count))
            {
                this.stat[record.Vehicle] = count + 1;
            }
            else
            {
                this.stat[record.Vehicle] = 1;
            }
        }
    }
}