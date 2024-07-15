namespace Skender.Stock.Indicators;

public record AroonResult
(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : Reusable(Timestamp, Oscillator.Null2NaN());
