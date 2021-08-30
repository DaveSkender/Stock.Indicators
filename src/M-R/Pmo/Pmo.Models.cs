using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class PmoResult : ResultBase
    {
        public decimal? Pmo { get; set; }
        public decimal? Signal { get; set; }

        // internal use only
        internal decimal? RocEma { get; set; }
    }
}