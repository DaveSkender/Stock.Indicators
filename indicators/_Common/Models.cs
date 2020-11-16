using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class Quote
    {
        internal int? Index { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }

    [Serializable]
    public class ResultBase
    {
        public DateTime Date { get; set; }
    }

    [Serializable]
    internal class BasicData
    {
        internal int? Index { get; set; }
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }
}
