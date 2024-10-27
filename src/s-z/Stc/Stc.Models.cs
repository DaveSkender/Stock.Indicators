namespace Skender.Stock.Indicators;

public record StcResult
(
    DateTime Timestamp,
    double? Stc
) : IReusable
{
    public double Value => Stc.Null2NaN();
}
