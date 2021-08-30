using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class WmaResult : ResultBase
    {
        public decimal? Wma { get; set; }
    }
}