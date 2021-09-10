using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StarcBandsResult : ResultBase
    {
        public decimal? UpperBand { get; set; }
        public decimal? Centerline { get; set; }
        public decimal? LowerBand { get; set; }
    }
}