namespace Skender.Stock.Indicators;

public readonly record struct KamaResult
(
    DateTime Timestamp,
    double? Er,
    double? Kama
) : IReusable
{
    double IReusable.Value => Kama.Null2NaN();
}
