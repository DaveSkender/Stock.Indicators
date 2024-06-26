namespace Skender.Stock.Indicators;

public record struct RsiResult(
    DateTime Timestamp,
    double? Rsi = null)
     : IReusableResult
{
    readonly double IReusableResult.Value
        => Rsi.Null2NaN();
}
