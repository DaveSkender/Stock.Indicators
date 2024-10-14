namespace Skender.Stock.Indicators;

public record HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : IReusable
{
    public double Value => HurstExponent.Null2NaN();
}
