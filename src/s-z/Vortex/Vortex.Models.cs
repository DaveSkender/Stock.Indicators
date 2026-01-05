namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Vortex indicator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Pvi">The positive vortex indicator value at this point.</param>
/// <param name="Nvi">The negative vortex indicator value at this point.</param>
[Serializable]
public record VortexResult
(
    DateTime Timestamp,
    double? Pvi = null,
    double? Nvi = null
) : IReusable
{
    public double Value => double.NaN;
}
