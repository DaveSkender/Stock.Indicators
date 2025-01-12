/// <summary>
/// Represents the result of a Simple Moving Average (SMA) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Sma">The value of the SMA at this point.</param>
[Serializable]
public record SmaResult(
    DateTime Timestamp,
    double? Sma
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Sma.Null2NaN();
}
