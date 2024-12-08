namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Relative Strength Index (RSI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the RSI result.</param>
/// <param name="Rsi">The RSI value.</param>
[Serializable]
public record RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Rsi.Null2NaN();
}
