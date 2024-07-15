namespace Skender.Stock.Indicators;

public record StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi = null,
    double? Signal = null
) : Reusable(Timestamp, StochRsi.Null2NaN());
