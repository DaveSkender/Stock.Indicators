namespace Skender.Stock.Indicators;

[Serializable]
public record VwapResult
(
    DateTime Timestamp,
    double? Vwap
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Vwap.Null2NaN();
}
