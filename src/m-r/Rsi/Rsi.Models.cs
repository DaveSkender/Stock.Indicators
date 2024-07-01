namespace Skender.Stock.Indicators;

public readonly record struct RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : IReusable
{
    double IReusable.Value => Rsi.Null2NaN();
}
