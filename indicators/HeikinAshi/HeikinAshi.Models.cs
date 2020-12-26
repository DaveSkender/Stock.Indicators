using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class HeikinAshiResult : ResultBase
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}