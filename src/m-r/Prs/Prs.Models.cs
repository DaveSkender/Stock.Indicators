namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Price Relative Strength (PRS) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the PRS result.</param>
/// <param name="Prs">The PRS value.</param>
/// <param name="PrsPercent">The PRS percentage value.</param>
[Serializable]
public record PrsResult
(
    DateTime Timestamp,
    double? Prs,
    double? PrsPercent
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Prs.Null2NaN();
}
