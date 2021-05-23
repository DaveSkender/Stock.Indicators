using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RsiResult : ResultBase
    {
        public decimal? Rsi { get; set; }
    }

    public class RsiExtendedResult : RsiResult
    {
        public decimal? BullishPrice { get; set; }
        public decimal? BearishPrice { get; set; }
        public decimal? BullishRsi { get; set; }
        public decimal? BearishRsi { get; set; }
        public DivergenceType? DivergenceType { get; set; }
    }
}
