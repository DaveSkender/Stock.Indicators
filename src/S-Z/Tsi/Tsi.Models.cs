using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class TsiResult : ResultBase
    {
        public decimal? Tsi { get; set; }
        public decimal? Signal { get; set; }
    }
}