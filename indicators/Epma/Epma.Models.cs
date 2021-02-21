using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class EpmaResult : ResultBase
    {
        public decimal? Epma { get; set; }
    }
}
