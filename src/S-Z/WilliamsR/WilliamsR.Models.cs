using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class WilliamsResult : ResultBase
    {
        public decimal? WilliamsR { get; set; }
    }
}