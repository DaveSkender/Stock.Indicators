namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Chandelier Exit calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="ChandelierExit">The Chandelier Exit value.</param>
[Serializable]
public record ChandelierResult
(
    DateTime Timestamp,
    double? ChandelierExit
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => ChandelierExit.Null2NaN();
}
