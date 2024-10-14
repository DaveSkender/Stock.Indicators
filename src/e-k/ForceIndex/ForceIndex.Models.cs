namespace Skender.Stock.Indicators;

public record ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex = null
) : IReusable
{
    public double Value => ForceIndex.Null2NaN();
}
