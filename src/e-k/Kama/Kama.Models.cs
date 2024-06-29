namespace Skender.Stock.Indicators;

public readonly record struct KamaResult
(
    DateTime Timestamp,
    double? ER,
    double? Kama
) : IReusable
{
    double IReusable.Value => Kama.Null2NaN();
}
