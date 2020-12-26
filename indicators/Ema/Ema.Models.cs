using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class EmaResult : ResultBase
    {
        public decimal? Ema { get; set; }
    }
}