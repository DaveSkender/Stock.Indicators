using System;

namespace Skender.Stock.Indicators
{

    public class AtrResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Tr { get; set; }
        public decimal? Atr { get; set; }
        public decimal? Atrp { get; set; }
    }

}
