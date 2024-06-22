namespace Skender.Stock.Indicators;

public record struct AdxResult(
    DateTime Timestamp,
    double? Pdi = default,
    double? Mdi = default,
    double? Adx = default,
    double? Adxr = default)
: IReusableResult
{
    readonly double IReusableResult.Value
        => Adx.Null2NaN();
}
