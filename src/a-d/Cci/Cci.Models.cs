namespace Skender.Stock.Indicators;

[Serializable]
public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Cci.Null2NaN();
}
