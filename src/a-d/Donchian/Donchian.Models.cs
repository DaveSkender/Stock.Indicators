namespace Skender.Stock.Indicators;

public record DonchianResult
(
    DateTime Timestamp,
    decimal? UpperBand = null,
    decimal? Centerline = null,
    decimal? LowerBand = null,
    decimal? Width = null
) : ISeries;
