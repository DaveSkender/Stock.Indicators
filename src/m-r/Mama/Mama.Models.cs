namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a MAMA (MESA Adaptive Moving Average) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Mama">The value of the MAMA.</param>
/// <param name="Fama">The value of the FAMA.</param>
[Serializable]
public record MamaResult
(
    DateTime Timestamp,
    double? Mama = null,
    double? Fama = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Mama.Null2NaN();
}
