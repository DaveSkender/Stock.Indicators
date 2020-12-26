using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StochResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Signal { get; set; }

        // internal use only
        internal decimal? Smooth { get; set; }
    }
}