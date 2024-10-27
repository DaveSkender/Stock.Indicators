namespace Skender.Stock.Indicators;

public record AlmaResult
(
    DateTime Timestamp,
    double? Alma
) : IReusable
{
    public double Value => Alma.Null2NaN();
}
