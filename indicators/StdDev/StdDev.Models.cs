using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StdDevResult : ResultBase
    {
        public decimal? StdDev { get; set; }
        public decimal? ZScore { get; set; }
        public decimal? Sma { get; set; }
    }
}
