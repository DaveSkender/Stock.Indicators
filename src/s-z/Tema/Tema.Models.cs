namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Triple Exponential Moving Average (TEMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Tema">The value of the TEMA at this point.</param>
[Serializable]
public record TemaResult
(
    DateTime Timestamp,
    double? Tema = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Tema.Null2NaN();
}
