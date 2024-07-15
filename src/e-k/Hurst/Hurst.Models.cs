namespace Skender.Stock.Indicators;

public record HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : Reusable(Timestamp, HurstExponent.Null2NaN());
