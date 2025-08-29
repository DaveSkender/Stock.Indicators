namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Chaikin Money Flow (CMF) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="MoneyFlowMultiplier">The money flow multiplier value.</param>
/// <param name="MoneyFlowVolume">The money flow volume value.</param>
/// <param name="Cmf">The Chaikin Money Flow value.</param>
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
    [JsonIgnore]
    public double Value => Cmf.Null2NaN();
}
