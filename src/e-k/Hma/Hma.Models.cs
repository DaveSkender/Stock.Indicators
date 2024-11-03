namespace Skender.Stock.Indicators;

[Serializable]
public record HmaResult
(
    DateTime Timestamp,
    double? Hma = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Hma.Null2NaN();
}
