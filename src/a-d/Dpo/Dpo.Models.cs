namespace Skender.Stock.Indicators;

public record struct DpoResult(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null)
    : IReusable
{
    readonly double IReusable.Value
        => Dpo.Null2NaN();
}
