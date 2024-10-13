namespace Skender.Stock.Indicators;

public record EmaResult
(
    DateTime Timestamp,
    double? Ema = null
) : IReusable
{
    public double Value => Ema.Null2NaN();
}
