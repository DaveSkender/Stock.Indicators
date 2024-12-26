namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Money Flow Index (MFI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Mfi">The value of the Money Flow Index (MFI).</param>
[Serializable]
public record MfiResult
(
   DateTime Timestamp,
   double? Mfi
) : IReusable
{
    /// <inheritdoc/>
    [JsonIgnore]
    public double Value => Mfi.Null2NaN();
}
