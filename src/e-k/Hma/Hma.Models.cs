namespace Skender.Stock.Indicators;

public record HmaResult
(
    DateTime Timestamp,
    double? Hma = null
) : Reusable(Timestamp, Hma.Null2NaN());
