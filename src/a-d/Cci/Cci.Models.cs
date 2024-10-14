namespace Skender.Stock.Indicators;

public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : IReusable
{
    public double Value => Cci.Null2NaN();
}
