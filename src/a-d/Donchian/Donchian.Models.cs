namespace Skender.Stock.Indicators;

public readonly record struct DonchianResult
(
    DateTime Timestamp,
    decimal? UpperBand,
    decimal? Centerline,
    decimal? LowerBand,
    decimal? Width
) : IResult;
