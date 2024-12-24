namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of an Exponential Polynomial Moving Average (EPMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Epma">The EPMA value.</param>
[Serializable]
public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Epma.Null2NaN();
}
