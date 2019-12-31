using System;

namespace Skender.Stock.Indicators
{

    public class SmaResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Sma { get; set; }
    }

}
