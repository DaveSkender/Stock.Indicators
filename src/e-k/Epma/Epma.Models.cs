namespace Skender.Stock.Indicators;

public readonly record struct EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : IReusable
{
    double IReusable.Value => Epma.Null2NaN();
}
