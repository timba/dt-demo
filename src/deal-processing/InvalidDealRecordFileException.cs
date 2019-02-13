using System;

namespace DTDemo.DealProcessing
{
    [Serializable]
    public class InvalidDealRecordFileException : ApplicationException
    {
        public InvalidDealRecordFileException(string message) : base(message)
        {
        }
    }
}