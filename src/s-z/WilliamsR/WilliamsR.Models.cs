namespace Skender.Stock.Indicators;

[Serializable]
public record WilliamsResult
(
    DateTime Timestamp,
    double? WilliamsR
) : IReusable
{
    /// <inheritdoc/>
    public double Value => WilliamsR.Null2NaN();
}
