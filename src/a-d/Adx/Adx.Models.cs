namespace Skender.Stock.Indicators;

public readonly record struct AdxResult
(
    DateTime Timestamp,
    double? Pdi = null,
    double? Mdi = null,
    double? Adx = null,
    double? Adxr = null
) : IReusable
{
    double IReusable.Value => Adx.Null2NaN();
}
