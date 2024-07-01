namespace Skender.Stock.Indicators;

public readonly record struct ObvResult
(
    DateTime Timestamp,
    double Obv
) : IReusable
{
    double IReusable.Value => Obv;
}
