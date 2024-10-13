namespace Skender.Stock.Indicators;

public record StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : IReusable
{
    public double Value => StdDev.Null2NaN();
}
