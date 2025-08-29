namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Rate of Change (ROC) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the ROC result.</param>
/// <param name="Momentum">The momentum value of the ROC result.</param>
/// <param name="Roc">The rate of change value of the ROC result.</param>
[Serializable]
public record RocResult
(
    DateTime Timestamp,
    double? Momentum,
    double? Roc
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Roc.Null2NaN();
}
