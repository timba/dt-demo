using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing
{
    public interface IDealRecordStatAccumulator
    {
        (string, int)? GetMostOftenSoldVehicle();

        void Scan(DealRecord record);
    }
}