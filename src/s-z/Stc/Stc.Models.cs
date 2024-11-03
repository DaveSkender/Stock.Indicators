namespace Skender.Stock.Indicators;

[Serializable]
public record StcResult
(
    DateTime Timestamp,
    double? Stc
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Stc.Null2NaN();
}
