using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class GatorResult : ResultBase
    {
        public decimal? Upper { get; set; }
        public decimal? Lower { get; set; }

        public bool? UpperIsExpanding { get; set; }
        public bool? LowerIsExpanding { get; set; }
    }
}
