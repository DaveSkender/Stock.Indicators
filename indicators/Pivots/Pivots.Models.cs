using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class PivotsResult : ResultBase
    {
        public decimal? HighPoint { get; set; }
        public decimal? LowPoint { get; set; }
        public PivotTrend? HighTrend { get; set; }
        public PivotTrend? LowTrend { get; set; }
    }
}
