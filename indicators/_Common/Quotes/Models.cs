using System;

namespace Skender.Stock.Indicators
{
    // QUOTE MODELS

    public interface IQuote
    {
        public DateTime Date { get; }
        public decimal Open { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Close { get; }
        public decimal Volume { get; }
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
    internal class BasicData
    {
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }

    [Serializable]
    internal class Candle : Quote
    {
        // raw sizes
        internal decimal Size => High - Low;
        internal decimal RealBody => (Open > Close) ? (Open - Close) : (Close - Open);
        internal decimal UpperWick => High - (Open > Close ? Open : Close);
        internal decimal LowerWick => (Open > Close ? Close : Open) - Low;

        // percent sizes
        internal decimal RealBodyPct => (Size != 0) ? RealBody / Size : 1m;
        internal decimal UpperWickPct => (Size != 0) ? UpperWick / Size : 1m;
        internal decimal LowerWickPct => (Size != 0) ? LowerWick / Size : 1m;

        // directional info
        internal bool IsBullish => (Close > Open);
        internal bool IsBearish => (Close < Open);
    }
}
