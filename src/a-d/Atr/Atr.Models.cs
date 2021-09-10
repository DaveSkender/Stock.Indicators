using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AtrResult : ResultBase
    {
        public decimal? Tr { get; set; }
        public decimal? Atr { get; set; }
        public decimal? Atrp { get; set; }
    }
}