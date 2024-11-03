namespace Skender.Stock.Indicators;

[Serializable]
public record ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => ForceIndex.Null2NaN();
}
