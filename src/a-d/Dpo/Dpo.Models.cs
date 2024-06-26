namespace Skender.Stock.Indicators;

public record struct DpoResult(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null)
    : IReusableResult
{
    readonly double IReusableResult.Value
        => Dpo.Null2NaN();
}
