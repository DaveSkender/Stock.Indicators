namespace Skender.Stock.Indicators;

[Serializable]
public record ConnorsRsiResult
(
    DateTime Timestamp,
    double Streak,
    double? Rsi = null,
    double? RsiStreak = null,
    double? PercentRank = null,
    double? ConnorsRsi = null
) : IReusable
{
    public double Value => ConnorsRsi.Null2NaN();
}
