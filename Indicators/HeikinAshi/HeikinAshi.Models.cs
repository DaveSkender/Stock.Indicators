using System;

namespace Skender.Stock.Indicators
{

    public class HeikinAshiResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public bool? IsBullish { get; set; }
        public decimal Weakness { get; set; }
    }

}
