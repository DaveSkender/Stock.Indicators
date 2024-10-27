namespace Skender.Stock.Indicators;

[Serializable]
public record SmaResult(
    DateTime Timestamp,
    double? Sma
) : IReusable
{
    public double Value => Sma.Null2NaN();
}
