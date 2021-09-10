using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class FcbResult : ResultBase
    {
        public decimal? UpperBand { get; set; }
        public decimal? LowerBand { get; set; }
    }
}
