namespace Skender.Stock.Indicators;

public record struct AroonResult(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : IReusable
{
    readonly double IReusable.Value
        => Oscillator.Null2NaN();
}
