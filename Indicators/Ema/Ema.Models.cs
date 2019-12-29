using System;

namespace StockIndicators
{

    public class EmaResult
    {
        public DateTime Date { get; internal set; }
        public decimal? Ema { get; internal set; }
    }

}
