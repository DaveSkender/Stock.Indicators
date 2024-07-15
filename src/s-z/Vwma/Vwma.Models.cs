namespace Skender.Stock.Indicators;

public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : Reusable(Timestamp, Vwma.Null2NaN());
