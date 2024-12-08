namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the KAMA (Kaufman's Adaptive Moving Average) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Er">The Efficiency Ratio (ER) value.</param>
/// <param name="Kama">The KAMA value.</param>
[Serializable]
public record KamaResult
(
    DateTime Timestamp,
    double? Er = null,
    double? Kama = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Kama.Null2NaN();
}
