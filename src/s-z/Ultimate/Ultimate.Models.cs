namespace Skender.Stock.Indicators;

public record UltimateResult
(
    DateTime Timestamp,
    double? Ultimate
) : Reusable(Timestamp, Ultimate.Null2NaN());
