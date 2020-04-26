using System;

namespace Skender.Stock.Indicators
{

    public class BetaResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? Beta { get; set; }
    }

}
