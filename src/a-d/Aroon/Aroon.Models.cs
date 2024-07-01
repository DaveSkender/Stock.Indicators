namespace Skender.Stock.Indicators;

public readonly record struct AroonResult
(
    DateTime Timestamp,
    double? AroonUp,
    double? AroonDown,
    double? Oscillator
) : IReusable
{
    double IReusable.Value => Oscillator.Null2NaN();
}
