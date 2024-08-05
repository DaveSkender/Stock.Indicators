namespace Skender.Stock.Indicators;

public record StarcBandsResult
(
    DateTime Timestamp,
    double? UpperBand,
    double? Centerline,
    double? LowerBand
) : ISeries;
