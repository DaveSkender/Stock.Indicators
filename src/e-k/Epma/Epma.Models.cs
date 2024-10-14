namespace Skender.Stock.Indicators;

public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : IReusable
{
    public double Value => Epma.Null2NaN();
}
