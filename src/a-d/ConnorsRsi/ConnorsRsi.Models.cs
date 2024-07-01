namespace Skender.Stock.Indicators;

public readonly record struct ConnorsRsiResult
(
    DateTime Timestamp,
    double Streak,
    double? Rsi,
    double? RsiStreak,
    double? PercentRank,
    double? ConnorsRsi
) : IReusable
{
    double IReusable.Value => ConnorsRsi.Null2NaN();
}
