namespace Skender.Stock.Indicators;

public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : IReusable
{
    public double Value => Vwma.Null2NaN();
}
