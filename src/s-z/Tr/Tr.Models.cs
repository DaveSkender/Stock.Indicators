namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a True Range (TR) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Tr">The value of the True Range at this point.</param>
[Serializable]
public record TrResult(
    DateTime Timestamp,
    double? Tr
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Tr.Null2NaN();
}
