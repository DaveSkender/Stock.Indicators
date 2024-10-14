namespace Skender.Stock.Indicators;

public record DemaResult
(
    DateTime Timestamp,
    double? Dema = null
) : IReusable
{
    public double Value => Dema.Null2NaN();
}
