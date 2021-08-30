using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class SmmaResult : ResultBase
    {
        public decimal? Smma { get; set; }
    }
}
