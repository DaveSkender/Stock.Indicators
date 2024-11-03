namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Awesome Oscillator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Oscillator">The value of the Awesome Oscillator.</param>
/// <param name="Normalized">The normalized value of the Awesome Oscillator.</param>
[Serializable]
public record AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Oscillator.Null2NaN();
}
