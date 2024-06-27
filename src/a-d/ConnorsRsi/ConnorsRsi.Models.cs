namespace Skender.Stock.Indicators;

public record struct ConnorsRsiResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Rsi { get; set; }
    public double? RsiStreak { get; set; }
    public double? PercentRank { get; set; }
    public double? ConnorsRsi { get; set; }

    // internal use only
    internal double Streak { get; set; }

    readonly double IReusable.Value
        => ConnorsRsi.Null2NaN();
}
