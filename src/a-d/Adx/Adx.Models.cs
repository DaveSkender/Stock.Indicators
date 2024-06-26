namespace Skender.Stock.Indicators;

public record struct AdxResult(
    DateTime Timestamp,
    double? Pdi = null,
    double? Mdi = null,
    double? Adx = null,
    double? Adxr = null)
: IReusableResult
{
    readonly double IReusableResult.Value
        => Adx.Null2NaN();
}
