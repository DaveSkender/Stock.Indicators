namespace Skender.Stock.Indicators;

public record StochRsiResult
(
    DateTime Timestamp,
    double? StochRsi = null,
    double? Signal = null
) : IReusable
{
    public double Value => StochRsi.Null2NaN();
}
