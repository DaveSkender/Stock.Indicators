namespace Skender.Stock.Indicators;

public record WilliamsResult
(
    DateTime Timestamp,
    double? WilliamsR
) : Reusable(Timestamp, WilliamsR.Null2NaN());
