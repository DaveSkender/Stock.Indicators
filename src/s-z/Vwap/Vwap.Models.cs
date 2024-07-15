namespace Skender.Stock.Indicators;

public record VwapResult
(
    DateTime Timestamp,
    double? Vwap
) : Reusable(Timestamp, Vwap.Null2NaN());
