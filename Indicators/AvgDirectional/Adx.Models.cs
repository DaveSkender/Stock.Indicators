using System;

namespace Skender.Stock.Indicators
{

    public class AdxResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Pdi { get; set; }
        public decimal? Mdi { get; set; }
        public decimal? Adx { get; set; }
    }

}
