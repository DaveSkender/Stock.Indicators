namespace Skender.Stock.Indicators;

public record SmmaResult
(
    DateTime Timestamp,
    double? Smma = null
) : Reusable(Timestamp, Smma.Null2NaN());
