namespace Skender.Stock.Indicators;

public record RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : Reusable(Timestamp, Rsi.Null2NaN());
