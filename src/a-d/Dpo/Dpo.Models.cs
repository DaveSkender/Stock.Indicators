namespace Skender.Stock.Indicators;

public record struct DpoResult(
    DateTime Timestamp,
    double? Dpo = default,
    double? Sma = default)
    : IReusableResult
{
    readonly double IReusableResult.Value
        => Dpo.Null2NaN();
}
