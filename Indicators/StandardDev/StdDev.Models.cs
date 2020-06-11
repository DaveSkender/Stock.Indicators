using System;

namespace Skender.Stock.Indicators
{

    public class StdDevResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? StdDev { get; set; }
        public decimal? ZScore { get; set; }
    }

}
