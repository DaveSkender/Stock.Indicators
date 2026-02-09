namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Force Index calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="ForceIndex">The Force Index value.</param>
[Serializable]
public record ForceIndexResult
(
    DateTime Timestamp,
    double? ForceIndex = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => ForceIndex.Null2NaN();
}
