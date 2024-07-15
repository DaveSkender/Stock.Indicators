namespace Skender.Stock.Indicators;

public record RocResult
(
    DateTime Timestamp,
    double? Momentum,
    double? Roc
) : Reusable(Timestamp, Roc.Null2NaN());
