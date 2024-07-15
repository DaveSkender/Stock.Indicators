namespace Skender.Stock.Indicators;

public record SmaResult(
    DateTime Timestamp,
    double? Sma
) : Reusable(Timestamp, Sma.Null2NaN());
