using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class KeltnerResult : ResultBase
    {
        public decimal? UpperBand { get; set; }
        public decimal? Centerline { get; set; }
        public decimal? LowerBand { get; set; }
        public decimal? Width { get; set; }
    }
}