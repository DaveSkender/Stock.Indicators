/// <summary>
/// Represents the result of a DEMA (Double Exponential Moving Average) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the DEMA result.</param>
/// <param name="Dema">The DEMA value.</param>
[Serializable]
public record DemaResult
(
    DateTime Timestamp,
    double? Dema = null
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Dema.Null2NaN();
}
