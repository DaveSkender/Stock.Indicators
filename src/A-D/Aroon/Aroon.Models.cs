using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AroonResult : ResultBase
    {
        public decimal? AroonUp { get; set; }
        public decimal? AroonDown { get; set; }
        public decimal? Oscillator { get; set; }
    }
}