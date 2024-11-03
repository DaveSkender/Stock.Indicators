namespace Skender.Stock.Indicators;

[Serializable]
public record PmoResult
(
    DateTime Timestamp,
    double? Pmo,
    double? Signal
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Pmo.Null2NaN();
}
