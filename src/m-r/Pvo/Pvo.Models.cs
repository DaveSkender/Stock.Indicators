using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class PvoResult : ResultBase
    {
        public decimal? Pvo { get; set; }
        public decimal? Signal { get; set; }
        public decimal? Histogram { get; set; }
    }
}