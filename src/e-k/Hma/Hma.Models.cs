namespace Skender.Stock.Indicators;

public record HmaResult
(
    DateTime Timestamp,
    double? Hma = null
) : IReusable
{
    public double Value => Hma.Null2NaN();
}
