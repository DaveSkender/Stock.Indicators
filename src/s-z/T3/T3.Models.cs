namespace Skender.Stock.Indicators;

public record T3Result
(
    DateTime Timestamp,
    double? T3
) : Reusable(Timestamp, T3.Null2NaN());
