namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Connors RSI calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Streak">The streak value.</param>
/// <param name="Rsi">The RSI value.</param>
/// <param name="RsiStreak">The RSI streak value.</param>
/// <param name="PercentRank">The percent rank value.</param>
/// <param name="ConnorsRsi">The Connors RSI value.</param>
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
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => ConnorsRsi.Null2NaN();
}
