using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RocResult : ResultBase
    {
        public decimal? Roc { get; set; }
        public decimal? RocSma { get; set; }
    }

    [Serializable]
    public class RocWbResult : ResultBase
    {
        public decimal? Roc { get; set; }
        public decimal? RocEma { get; set; }
        public decimal? UpperBand { get; set; }
        public decimal? LowerBand { get; set; }
    }
}
