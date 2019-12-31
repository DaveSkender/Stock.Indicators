using System;

namespace Skender.Stock.Indicators
{

    public class EmaResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Ema { get; set; }
    }

}
