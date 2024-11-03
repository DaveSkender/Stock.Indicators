namespace Skender.Stock.Indicators;

[Serializable]
public record UlcerIndexResult
(
    DateTime Timestamp,
    double? UlcerIndex
) : IReusable
{
    /// <inheritdoc/>
    public double Value => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")] // v3.0.0
    public double? UI => UlcerIndex;
}
