namespace Skender.Stock.Indicators;

public readonly record struct StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi,
    double? Signal
) : IReusable
{
    double IReusable.Value => StochRsi.Null2NaN();
}
