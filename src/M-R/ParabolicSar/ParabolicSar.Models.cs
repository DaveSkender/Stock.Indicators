using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ParabolicSarResult : ResultBase
    {
        public decimal? Sar { get; set; }
        public bool? IsReversal { get; set; }
    }
}