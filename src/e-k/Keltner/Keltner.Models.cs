namespace Skender.Stock.Indicators;

public record KeltnerResult
(
    DateTime Timestamp,
    double? UpperBand = null,
    double? Centerline = null,
    double? LowerBand = null,
    double? Width = null
) : ISeries;
