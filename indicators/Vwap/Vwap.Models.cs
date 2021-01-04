using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class VwapResult : ResultBase
    {
        public decimal? Vwap { get; set; }
    }
}