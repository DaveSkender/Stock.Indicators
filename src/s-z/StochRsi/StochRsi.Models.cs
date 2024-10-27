namespace Skender.Stock.Indicators;

[Serializable]
public record StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi = null,
    double? Signal = null
) : IReusable
{
    public double Value => StochRsi.Null2NaN();
}
