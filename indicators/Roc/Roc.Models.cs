using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RocResult : ResultBase
    {
        public decimal? Roc { get; set; }
        public decimal? RocSma { get; set; }
    }
}