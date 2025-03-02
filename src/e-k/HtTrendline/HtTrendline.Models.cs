namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Hilbert Transform Trendline (HTL) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="DcPeriods">The dominant cycle periods.</param>
/// <param name="Trendline">The value of the trendline.</param>
/// <param name="SmoothPrice">The smoothed price.</param>
[Serializable]
public record HtlResult
(
    DateTime Timestamp,
    int? DcPeriods,
    double? Trendline,
    double? SmoothPrice
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Trendline.Null2NaN();
}
