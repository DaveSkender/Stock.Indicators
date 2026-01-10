namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Gator Oscillator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Upper">The upper value of the Gator Oscillator.</param>
/// <param name="Lower">The lower value of the Gator Oscillator.</param>
/// <param name="UpperIsExpanding">Indicates if the upper value is expanding.</param>
/// <param name="LowerIsExpanding">Indicates if the lower value is expanding.</param>
[Serializable]
public record GatorResult
(
    DateTime Timestamp,
    double? Upper = null,
    double? Lower = null,
    bool? UpperIsExpanding = null,
    bool? LowerIsExpanding = null
) : IReusable
{
    public double Value => double.NaN;
}
