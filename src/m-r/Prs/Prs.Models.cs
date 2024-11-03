namespace Skender.Stock.Indicators;

[Serializable]
public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Prs.Null2NaN();
}
