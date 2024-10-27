namespace Skender.Stock.Indicators;

public record RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : IReusable
{
    public double Value => Rsi.Null2NaN();
}
