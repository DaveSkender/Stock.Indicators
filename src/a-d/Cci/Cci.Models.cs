namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Commodity Channel Index (CCI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Cci">The Commodity Channel Index value.</param>
[Serializable]
public record CciResult
(
    DateTime Timestamp,
    double? Cci
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Cci.Null2NaN();
}
