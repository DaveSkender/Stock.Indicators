using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StochRsiResult : ResultBase
    {
        public decimal? StochRsi { get; set; }
        public decimal? Signal { get; set; }
    }
}