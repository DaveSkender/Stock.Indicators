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
        internal int? Index { get; set; }
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }
}
