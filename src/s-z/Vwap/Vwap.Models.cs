namespace Skender.Stock.Indicators;

public record VwapResult
(
    DateTime Timestamp,
    double? Vwap
) : IReusable
{
    public double Value => Vwap.Null2NaN();
}
