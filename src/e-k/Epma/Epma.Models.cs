namespace Skender.Stock.Indicators;

[Serializable]
public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : IReusable
{
    public double Value => Epma.Null2NaN();
}
