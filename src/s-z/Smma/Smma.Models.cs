namespace Skender.Stock.Indicators;

public record SmmaResult
(
    DateTime Timestamp,
    double? Smma = null
) : IReusable
{
    public double Value => Smma.Null2NaN();
}
