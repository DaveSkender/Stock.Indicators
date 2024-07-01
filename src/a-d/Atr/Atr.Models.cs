namespace Skender.Stock.Indicators;

public readonly record struct AtrResult
(
    DateTime Timestamp,
    double? Tr = null,
    double? Atr = null,
    double? Atrp = null
) : IReusable
{
    double IReusable.Value => Atrp.Null2NaN();
}
