namespace Skender.Stock.Indicators;

public readonly record struct PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : IReusable
{
    double IReusable.Value => Pmo.Null2NaN();
}
