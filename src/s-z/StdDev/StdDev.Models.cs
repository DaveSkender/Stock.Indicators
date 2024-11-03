namespace Skender.Stock.Indicators;

[Serializable]
public record StdDevResult
(
    DateTime Timestamp,
    double? StdDev,
    double? Mean,
    double? ZScore
) : IReusable
{
    /// <inheritdoc/>
    public double Value => StdDev.Null2NaN();
}
