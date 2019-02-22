using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing
{
    public interface IDealRecordService
    {
        IObservable<DealRecord> GetDeals(TextReader reader);
    }
}