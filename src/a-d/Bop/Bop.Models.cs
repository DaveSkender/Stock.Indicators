using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class BopResult : ResultBase
    {
        public decimal? Bop { get; set; }
    }
}