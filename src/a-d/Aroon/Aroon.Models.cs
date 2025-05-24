namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Aroon indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="AroonUp">The Aroon Up value.</param>
/// <param name="AroonDown">The Aroon Down value.</param>
/// <param name="Oscillator">The Aroon Oscillator value.</param>
[Serializable]
public record AroonResult
(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Oscillator.Null2NaN();
}
