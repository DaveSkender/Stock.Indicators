namespace Skender.Stock.Indicators;

public record SmaResult(
    DateTime Timestamp,
    double? Sma
) : IReusable
{
    public double Value => Sma.Null2NaN();
}
