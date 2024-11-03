namespace Skender.Stock.Indicators;

[Serializable]
public record HurstResult
(
    DateTime Timestamp,
    double? HurstExponent
) : IReusable
{
    /// <inheritdoc/>
    public double Value => HurstExponent.Null2NaN();
}
