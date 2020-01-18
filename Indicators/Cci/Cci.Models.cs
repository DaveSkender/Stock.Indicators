using System;

namespace Skender.Stock.Indicators
{

    public class CciResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        internal decimal? Tp { get; set; }
        public decimal? Cci { get; set; }
    }

}
