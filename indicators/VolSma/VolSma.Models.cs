using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class VolSmaResult : Quote
    {
        public decimal? VolSma { get; set; }  // simple moving average of volume
    }
}
