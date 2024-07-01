namespace Skender.Stock.Indicators;

public readonly record struct GatorResult
(
    DateTime Timestamp,
    double? Upper,
    double? Lower,
    bool? UpperIsExpanding,
    bool? LowerIsExpanding
) : IResult;
