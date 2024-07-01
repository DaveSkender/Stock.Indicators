namespace Skender.Stock.Indicators;

public readonly record struct KeltnerResult
(
    DateTime Timestamp,
    double? UpperBand,
    double? Centerline,
    double? LowerBand,
    double? Width
) : IResult;
