using System;

namespace DTDemo.DealProcessing
{
    [Serializable]
    public class ParseException : ApplicationException
    {
        public ParseException(string message) : base(message)
        {
        }

        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int Column { get; set; }

        public int Line { get; set; }
    }
}