namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ConnorsRsiResult : ResultBase
    {
        public double? RsiClose { get; set; }
        public double? RsiStreak { get; set; }
        public double? PercentRank { get; set; }
        public double? ConnorsRsi { get; set; }

        // internal use only
        internal int? Streak { get; set; }
    }
}
