namespace Skender.Stock.Indicators;

public record ConnorsRsiResult
(
    DateTime Timestamp,
    double Streak,
    double? Rsi = null,
    double? RsiStreak = null,
    double? PercentRank = null,
    double? ConnorsRsi = null
) : Reusable(Timestamp, ConnorsRsi.Null2NaN());
