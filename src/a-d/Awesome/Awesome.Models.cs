namespace Skender.Stock.Indicators;

public record AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : Reusable(Timestamp, Oscillator.Null2NaN());
