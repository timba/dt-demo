using System;

namespace DTDemo.DealProcessing
{
    public class DealRecord
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string DealershipName { get; set; }
        public string Vehicle { get; set; }
        public float Price { get; set; }
        public DateTime Date { get; set; }
    }
}
