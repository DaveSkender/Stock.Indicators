using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class KvoResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Signal { get; set; }
    }
}
