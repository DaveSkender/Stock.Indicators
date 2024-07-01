namespace Skender.Stock.Indicators;

public readonly record struct AwesomeResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Normalized
) : IReusable
{
    double IReusable.Value => Oscillator.Null2NaN();
}
