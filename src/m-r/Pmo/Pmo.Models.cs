namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Price Momentum Oscillator (PMO) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the PMO result.</param>
/// <param name="Pmo">The PMO value.</param>
/// <param name="Signal">The signal line value.</param>
[Serializable]
public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Pmo.Null2NaN();
}
