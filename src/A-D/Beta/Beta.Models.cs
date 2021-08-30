using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class BetaResult : ResultBase
    {
        public decimal? Beta { get; set; }
    }
}