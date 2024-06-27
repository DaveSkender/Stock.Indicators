namespace Skender.Stock.Indicators;

public record struct RsiResult(
    DateTime Timestamp,
    double? Rsi = null)
     : IReusable
{
    readonly double IReusable.Value
        => Rsi.Null2NaN();
}
