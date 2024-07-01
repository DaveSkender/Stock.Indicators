namespace Skender.Stock.Indicators;

public readonly record struct DemaResult
(
    DateTime Timestamp,
    double? Dema
) : IReusable
{
    double IReusable.Value => Dema.Null2NaN();
}
