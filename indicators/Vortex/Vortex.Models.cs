using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class VortexResult : ResultBase
    {
        public decimal? Pvi { get; set; }
        public decimal? Nvi { get; set; }
    }
}
