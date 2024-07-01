namespace Skender.Stock.Indicators;

public readonly record struct TrixResult
(
    DateTime Timestamp,
    double? Ema3,
    double? Trix
) : IReusable
{
    double IReusable.Value => Trix.Null2NaN();
}
