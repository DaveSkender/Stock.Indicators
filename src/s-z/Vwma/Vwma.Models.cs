namespace Skender.Stock.Indicators;

[Serializable]
public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : IReusable
{
    public double Value => Vwma.Null2NaN();
}
