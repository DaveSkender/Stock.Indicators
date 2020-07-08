using System;

namespace Skender.Stock.Indicators
{

    public class Quote
    {
        public int? Index { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }

    public class ResultBase
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
    }

    internal class BasicData
    {
        public int? Index { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
