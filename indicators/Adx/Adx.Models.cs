using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AdxResult : ResultBase
    {
        public decimal? Pdi { get; set; }
        public decimal? Mdi { get; set; }
        public decimal? Adx { get; set; }
    }
}