namespace Skender.Stock.Indicators;

[Serializable]
public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Vwma.Null2NaN();
}
