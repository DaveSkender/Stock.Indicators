namespace Skender.Stock.Indicators;

public readonly record struct TsiResult
(
    DateTime Timestamp,
    double? Tsi,
    double? Signal
) : IReusable
{
    double IReusable.Value => Tsi.Null2NaN();
}
