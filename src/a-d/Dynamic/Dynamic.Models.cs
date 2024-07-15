namespace Skender.Stock.Indicators;

public record DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : Reusable(Timestamp, Dynamic.Null2NaN());
