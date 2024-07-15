namespace Skender.Stock.Indicators;

public record ChopResult
(
    DateTime Timestamp,
    double? Chop
) : Reusable(Timestamp, Chop.Null2NaN());
