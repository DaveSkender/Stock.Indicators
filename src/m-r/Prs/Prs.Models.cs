using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class PrsResult : ResultBase
    {
        public decimal? Prs { get; set; }
        public decimal? PrsSma { get; set; }
        public decimal? PrsPercent { get; set; }
    }
}