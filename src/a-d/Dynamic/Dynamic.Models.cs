namespace Skender.Stock.Indicators;

public record DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : IReusable
{
    public double Value => Dynamic.Null2NaN();
}
