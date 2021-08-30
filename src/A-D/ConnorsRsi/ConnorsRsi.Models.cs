using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ConnorsRsiResult : ResultBase
    {
        public decimal? RsiClose { get; set; }
        public decimal? RsiStreak { get; set; }
        public decimal? PercentRank { get; set; }
        public decimal? ConnorsRsi { get; set; }

        // internal use only
        internal decimal? Streak { get; set; }
    }
}