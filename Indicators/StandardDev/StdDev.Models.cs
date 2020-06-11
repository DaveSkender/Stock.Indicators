using System;

namespace Skender.Stock.Indicators
{

    public class StdDevResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? StdDev { get; set; }
        public decimal? ZScore { get; set; }
        public decimal? AvgStdDev { get; set; }
        public decimal? StdDevPercent { get; set; }
        public decimal? StdDevChange { get; set; }
        public decimal? AvgStdDevChange { get; set; }

        // internal use only
        internal decimal? AvgClose { get; set; }
        internal decimal? Close { get; set; }
        internal decimal? PrevClose { get; set; }
    }

}
