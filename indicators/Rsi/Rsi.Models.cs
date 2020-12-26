using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RsiResult : ResultBase
    {
        public decimal? Rsi { get; set; }
    }
}