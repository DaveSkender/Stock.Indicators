namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the KVO (Klinger Volume Oscillator) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Oscillator">The oscillator value of the KVO.</param>
/// <param name="Signal">The signal value of the KVO.</param>
[Serializable]
public record KvoResult
(
    DateTime Timestamp,
    double? Oscillator = null,
    double? Signal = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Oscillator.Null2NaN();
}
