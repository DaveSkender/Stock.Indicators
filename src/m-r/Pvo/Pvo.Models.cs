namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Percentage Volume Oscillator (PVO) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the PVO result.</param>
/// <param name="Pvo">The PVO value.</param>
/// <param name="Signal">The signal line value.</param>
/// <param name="Histogram">The histogram value.</param>
[Serializable]
public record PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Pvo.Null2NaN();
}
