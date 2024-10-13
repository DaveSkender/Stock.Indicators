namespace Skender.Stock.Indicators;

public record WilliamsResult
(
    DateTime Timestamp,
    double? WilliamsR
) : IReusable
{
    public double Value => WilliamsR.Null2NaN();
}
