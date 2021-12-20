using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class VortexResult : ResultBase
    {
        public double? Pvi { get; set; }
        public double? Nvi { get; set; }
    }
}
