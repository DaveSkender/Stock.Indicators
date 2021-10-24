using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class BetaResult : ResultBase
    {
        public decimal? Beta { get; set; }
        public decimal? BetaUp { get; set; }
        public decimal? BetaDown { get; set; }
        public decimal? Ratio { get; set; }
        public decimal? Convexity { get; set; }
    }
}
