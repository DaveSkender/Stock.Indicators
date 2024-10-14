namespace Skender.Stock.Indicators;

public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : IReusable
{
    public double Value => Wma.Null2NaN();
}
