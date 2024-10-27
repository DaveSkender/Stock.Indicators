namespace Skender.Stock.Indicators;

[Serializable]
public record RocResult
(
    DateTime Timestamp,
    double? Momentum,
    double? Roc
) : IReusable
{
    public double Value => Roc.Null2NaN();
}
