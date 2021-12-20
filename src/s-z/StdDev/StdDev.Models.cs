using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StdDevResult : ResultBase
    {
        public double? StdDev { get; set; }
        public double? Mean { get; set; }
        public double? ZScore { get; set; }
        public double? StdDevSma { get; set; }
    }
}
