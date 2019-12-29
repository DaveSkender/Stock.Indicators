using System;

namespace StockIndicators
{

    public class HeikinAshiResult
    {
        public DateTime Date { get; internal set; }
        public decimal Open { get; internal set; }
        public decimal High { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal Close { get; internal set; }
        public bool IsBullish { get; internal set; }
        public decimal Weakness { get; internal set; }
    }

}
