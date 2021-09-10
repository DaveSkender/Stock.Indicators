using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class BollingerBandsResult : ResultBase
    {
        public decimal? Sma { get; set; }
        public decimal? UpperBand { get; set; }
        public decimal? LowerBand { get; set; }

        public decimal? PercentB { get; set; }
        public decimal? ZScore { get; set; }
        public decimal? Width { get; set; }
    }
}