using System;

namespace Skender.Stock.Indicators
{
    public interface IQuote
    {
        decimal Close { get; set; }
        DateTime Date { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Open { get; set; }
        decimal Volume { get; set; }
    }

    [Serializable]
    public class Quote : IQuote
    {
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
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }
}
