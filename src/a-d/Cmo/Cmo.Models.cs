namespace Skender.Stock.Indicators;

[Serializable]
public record CmoResult
(
    DateTime Timestamp,
    double? Cmo = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Cmo.Null2NaN();
}
