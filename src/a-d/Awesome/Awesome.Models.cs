namespace Skender.Stock.Indicators;

public record struct AwesomeResult(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : IReusable
{
    readonly double IReusable.Value
        => Oscillator.Null2NaN();
}
