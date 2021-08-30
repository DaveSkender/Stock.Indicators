using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StdDevResult : ResultBase
    {
        public decimal? StdDev { get; set; }
        public decimal? Mean { get; set; }
        public decimal? ZScore { get; set; }
        public decimal? StdDevSma { get; set; }
    }
}