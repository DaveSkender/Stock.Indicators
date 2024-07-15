namespace Skender.Stock.Indicators;

public record TrixResult
(
    DateTime Timestamp,
    double? Ema3 = null,
    double? Trix = null
) : Reusable(Timestamp, Trix.Null2NaN());
