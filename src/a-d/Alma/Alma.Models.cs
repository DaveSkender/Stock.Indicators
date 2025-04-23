namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the
/// ALMA (Arnaud Legoux Moving Average) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Alma">The ALMA value.</param>
[Serializable]
public record AlmaResult
(
    DateTime Timestamp,
    double? Alma
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Alma.Null2NaN();
}
