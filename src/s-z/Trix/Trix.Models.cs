using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class TrixResult : ResultBase
    {
        public decimal? Ema3 { get; set; }
        public decimal? Trix { get; set; }
        public decimal? Signal { get; set; }
    }
}