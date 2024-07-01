namespace Skender.Stock.Indicators;

public readonly record struct StdDevChannelsResult
(
    DateTime Timestamp,
    double? Centerline,
    double? UpperChannel,
    double? LowerChannel,
    bool BreakPoint
) : IResult;
