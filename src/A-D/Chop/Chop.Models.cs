using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ChopResult : ResultBase
    {
        public decimal? Chop { get; set; }
    }
}
