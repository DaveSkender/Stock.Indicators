using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class HtlResult : ResultBase
    {
        public decimal? Trendline { get; set; }
        public decimal? SmoothPrice { get; set; }
    }
}
