namespace Skender.Stock.Indicators;

[Serializable]
public record FisherTransformResult
(
    DateTime Timestamp,
    double? Fisher,
    double? Trigger
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Fisher.Null2NaN();
}
