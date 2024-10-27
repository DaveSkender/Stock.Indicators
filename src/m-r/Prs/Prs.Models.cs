namespace Skender.Stock.Indicators;

public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : IReusable
{
    public double Value => Prs.Null2NaN();
}
