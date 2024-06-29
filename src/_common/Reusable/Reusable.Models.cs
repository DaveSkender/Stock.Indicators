namespace Skender.Stock.Indicators;

public readonly record struct Reusable(
    DateTime Timestamp,
    double Value
) : IReusable;



