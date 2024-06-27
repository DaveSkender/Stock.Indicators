namespace Skender.Stock.Indicators;

public record struct AtrResult(
    DateTime Timestamp,
    double? Tr = null,
    double? Atr = null,
    double? Atrp = null
) : IReusable
{
    readonly double IReusable.Value
        => Atrp.Null2NaN();
}
