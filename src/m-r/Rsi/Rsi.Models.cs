namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Relative Strength Index (RSI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the RSI result.</param>
/// <param name="Rsi">The RSI value.</param>
/// <remarks>RSI is bounded from 0 to 100; precision handling preserves this range.</remarks>
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
