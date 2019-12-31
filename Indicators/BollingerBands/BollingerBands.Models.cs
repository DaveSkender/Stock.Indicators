using System;

namespace Skender.Stock.Indicators
{

    public class BollingerBandsResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Sma { get; set; }
        public decimal? UpperBand { get; set; }
        public decimal? LowerBand { get; set; }
        public bool? IsDiverging { get; set; }
    }

}
