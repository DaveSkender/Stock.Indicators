namespace Skender.Stock.Indicators;

[Serializable]
public record PvoResult
(
    DateTime Timestamp,
    double? Pvo,
    double? Signal,
    double? Histogram
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Pvo.Null2NaN();
}
