namespace Skender.Stock.Indicators;

[Serializable]
public record KeltnerResult
(
    DateTime Timestamp,
    double? UpperBand = null,
    double? Centerline = null,
    double? LowerBand = null,
    double? Width = null
) : ISeries;
