using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DTDemo.DealProcessing.Csv
{
    public interface IRecordParser
    {
        IObservable<(string[], int)> Parse(TextReader file);
    }
}