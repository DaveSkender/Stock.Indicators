namespace Skender.Stock.Indicators;

public readonly record struct KvoResult
(
    DateTime Timestamp,
    double? Oscillator,
    double? Signal
) : IReusable
{
    double IReusable.Value => Oscillator.Null2NaN();
}
