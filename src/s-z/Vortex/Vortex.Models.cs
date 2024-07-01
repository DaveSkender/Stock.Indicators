namespace Skender.Stock.Indicators;

public readonly record struct VortexResult
(
    DateTime Timestamp,
    double? Pvi,
    double? Nvi
) : IResult;
