using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class HmaResult : ResultBase
    {
        public decimal? Hma { get; set; }
    }
}