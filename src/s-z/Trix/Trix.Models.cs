namespace Skender.Stock.Indicators;

[Serializable]
public record TrixResult
(
    DateTime Timestamp,
    double? Ema3 = null,
    double? Trix = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Trix.Null2NaN();
}
