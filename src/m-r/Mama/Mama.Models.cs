namespace Skender.Stock.Indicators;

public readonly record struct MamaResult
(
    DateTime Timestamp,
    double? Mama,
    double? Fama
) : IReusable
{
    double IReusable.Value => Mama.Null2NaN();
}
