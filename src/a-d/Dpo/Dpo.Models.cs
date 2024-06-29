namespace Skender.Stock.Indicators;

public readonly record struct DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : IReusable
{
    double IReusable.Value => Dpo.Null2NaN();
}
