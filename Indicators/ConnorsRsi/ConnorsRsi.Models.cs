namespace Skender.Stock.Indicators
{

    public class ConnorsRsiResult : ResultBase
    {
        public float? RsiClose { get; set; }
        public float? RsiStreak { get; set; }
        public float? PercentRank { get; set; }
        public float? ConnorsRsi { get; set; }

        // internal use only
        internal int? Streak { get; set; }
        internal float? PeriodGain { get; set; }
    }

}
