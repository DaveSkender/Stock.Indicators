namespace Skender.Stock.Indicators;

[Serializable]
public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : IReusable
{
    public double Value => Prs.Null2NaN();
}
