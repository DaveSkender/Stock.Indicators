namespace Skender.Stock.Indicators;

public record TsiResult
(
    DateTime Timestamp,
    double? Tsi = null,
    double? Signal = null
) : IReusable
{
    public double Value => Tsi.Null2NaN();
}
