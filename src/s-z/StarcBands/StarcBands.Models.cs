namespace Skender.Stock.Indicators;

public readonly record struct StarcBandsResult
(
    DateTime Timestamp,
    double? UpperBand,
    double? Centerline,
    double? LowerBand
) : IResult;
