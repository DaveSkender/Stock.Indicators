namespace Skender.Stock.Indicators;

[Serializable]
public record DynamicResult
(
    DateTime Timestamp,
    double? Dynamic
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Dynamic.Null2NaN();
}
