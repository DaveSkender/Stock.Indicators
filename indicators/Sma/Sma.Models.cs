using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class SmaResult : ResultBase
    {
        public decimal? Sma { get; set; }  // simple moving average
        public decimal? Mad { get; set; }  // mean absolute deviation
        public decimal? Mse { get; set; }  // mean square error
        public decimal? Mape { get; set; } // mean absolute percentage error
    }
}