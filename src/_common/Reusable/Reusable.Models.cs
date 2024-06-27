namespace Skender.Stock.Indicators;

public record struct Reusable(
    DateTime Timestamp,
    double Value
) : IReusable;



