namespace Skender.Stock.Indicators;

public record ChopResult
(
    DateTime Timestamp,
    double? Chop
) : IReusable
{
    public double Value => Chop.Null2NaN();
}
