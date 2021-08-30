using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AwesomeResult : ResultBase
    {
        public decimal? Oscillator { get; set; }
        public decimal? Normalized { get; set; }
    }
}