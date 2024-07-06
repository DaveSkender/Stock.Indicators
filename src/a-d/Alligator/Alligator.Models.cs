namespace Skender.Stock.Indicators;

public readonly record struct AlligatorResult(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : IResult;

