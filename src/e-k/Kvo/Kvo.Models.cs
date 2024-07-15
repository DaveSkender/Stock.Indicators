namespace Skender.Stock.Indicators;

public record KvoResult
(
    DateTime Timestamp,
    double? Oscillator = null,
    double? Signal = null
) : Reusable(Timestamp, Oscillator.Null2NaN());
