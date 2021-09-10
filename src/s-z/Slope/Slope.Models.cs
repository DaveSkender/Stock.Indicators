using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class SlopeResult : ResultBase
    {
        public decimal? Slope { get; set; }
        public decimal? Intercept { get; set; }
        public double? StdDev { get; set; }
        public double? RSquared { get; set; }
        public decimal? Line { get; set; }         // last line segment only
    }
}