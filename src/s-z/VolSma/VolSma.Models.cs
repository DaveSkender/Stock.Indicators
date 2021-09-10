using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class VolSmaResult : ResultBase
    {
        public decimal Volume { get; set; }   // for reference only
        public decimal? VolSma { get; set; }  // simple moving average of volume
    }
}