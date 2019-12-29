using System;

namespace StockIndicators
{

    public class BollingerBandsResult
    {
        public DateTime Date { get; internal set; }
        public decimal? Sma { get; internal set; }
        public decimal? UpperBand { get; internal set; }
        public decimal? LowerBand { get; internal set; }
        public bool? IsDiverging { get; internal set; }
    }

}
