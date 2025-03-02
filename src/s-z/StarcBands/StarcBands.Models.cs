namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a STARC Bands calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="UpperBand">The value of the upper band at this point.</param>
/// <param name="Centerline">The value of the centerline at this point.</param>
/// <param name="LowerBand">The value of the lower band at this point.</param>
[Serializable]
public record StarcBandsResult
(
    DateTime Timestamp,
    double? UpperBand,
    double? Centerline,
    double? LowerBand
) : ISeries;
