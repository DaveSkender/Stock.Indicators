namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Balance of Power (BOP) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Bop">The Balance of Power value.</param>
[Serializable]
public record BopResult
(
    DateTime Timestamp,
    double? Bop
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Bop.Null2NaN();
}
