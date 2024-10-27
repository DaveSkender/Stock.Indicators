namespace Skender.Stock.Indicators;

[Serializable]
public record StarcBandsResult
(
    DateTime Timestamp,
    double? UpperBand,
    double? Centerline,
    double? LowerBand
) : ISeries;
