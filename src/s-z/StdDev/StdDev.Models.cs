namespace Skender.Stock.Indicators;

public record StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : Reusable(Timestamp, StdDev.Null2NaN());
