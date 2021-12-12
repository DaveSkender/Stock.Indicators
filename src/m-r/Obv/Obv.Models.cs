using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ObvResult : ResultBase
    {
        public double Obv { get; set; }
        public double? ObvSma { get; set; }
    }
}
