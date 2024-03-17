namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class ConnorsRsiResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Rsi { get; set; }
    public double? RsiStreak { get; set; }
    public double? PercentRank { get; set; }
    public double? ConnorsRsi { get; set; }

    // internal use only
    internal double Streak { get; set; }
    double IReusableResult.Value => ConnorsRsi.Null2NaN();
}
