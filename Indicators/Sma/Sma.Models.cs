using System;

namespace Skender.Stock.Indicators
{

    public class SmaResult
    {
        public DateTime Date { get; internal set; }
        public decimal? Sma { get; internal set; }
    }

}
