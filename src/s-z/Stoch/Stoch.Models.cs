using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StochResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Signal { get; set; }
        public decimal? PercentJ { get; set; }

        // aliases
        public decimal? K => Oscillator;
        public decimal? D => Signal;
        public decimal? J => PercentJ;
    }
}
