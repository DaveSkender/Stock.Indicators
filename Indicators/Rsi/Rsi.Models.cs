using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RsiResult : ResultBase
    {
        public decimal? Rsi { get; set; }
        public bool? IsIncreasing { get; set; }

        // internal use only
        internal decimal Gain { get; set; } = 0;
        internal decimal Loss { get; set; } = 0;
    }
}
