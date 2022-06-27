namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ConnorsRsiResult : ResultBase, IReusableResult
{
    public ConnorsRsiResult(DateTime date)
    {
        Date = date;
    }

    public double? Rsi { get; set; }
    public double? RsiStreak { get; set; }
    public double? PercentRank { get; set; }
    public double? ConnorsRsi { get; set; }

    // internal use only
    internal int Streak { get; set; }
    double? IReusableResult.Value => ConnorsRsi;
}
