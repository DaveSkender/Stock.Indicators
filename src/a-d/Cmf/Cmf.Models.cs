namespace Skender.Stock.Indicators;

[Serializable]
public record CmfResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Cmf
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Cmf.Null2NaN();
}
