namespace Skender.Stock.Indicators;

[Serializable]
public record TsiResult
(
    DateTime Timestamp,
    double? Tsi = null,
    double? Signal = null
) : IReusable
{
    public double Value => Tsi.Null2NaN();
}
