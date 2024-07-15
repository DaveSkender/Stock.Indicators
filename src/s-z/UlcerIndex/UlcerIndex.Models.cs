namespace Skender.Stock.Indicators;

public record UlcerIndexResult
(
    DateTime Timestamp,
    double? UlcerIndex  // ulcer index
) : Reusable(Timestamp, UlcerIndex.Null2NaN())
{
    [Obsolete("Rename UI to UlcerIndex")] // v3.0.0
    public double? UI => UlcerIndex;
}
