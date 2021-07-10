using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class HurstResult : ResultBase
    {
        public decimal? Hurst { get; set; }
    }
}
