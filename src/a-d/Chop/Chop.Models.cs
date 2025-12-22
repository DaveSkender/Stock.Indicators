namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Choppiness Index (CHOP) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Chop">The Choppiness Index value.</param>
/// <remarks>Choppiness Index is bounded from 0 to 100; precision handling preserves this range.</remarks>
[Serializable]
public record ChopResult
(
    DateTime Timestamp,
    double? Chop
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Chop.Null2NaN();
}
