namespace Skender.Stock.Indicators;

[Serializable]
public record DonchianResult
(
    DateTime Timestamp,
    decimal? UpperBand = null,
    decimal? Centerline = null,
    decimal? LowerBand = null,
    decimal? Width = null
) : ISeries;
