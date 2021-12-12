using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class BopResult : ResultBase
    {
        public double? Bop { get; set; }
    }
}
