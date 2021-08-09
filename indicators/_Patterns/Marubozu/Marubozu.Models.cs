using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class MarubozuResult : ResultBase
    {
        public decimal? Marubozu { get; set; }
        public bool? IsBullish { get; set; }
    }
}
