namespace Skender.Stock.Indicators;

public readonly record struct UlcerIndexResult
(
    DateTime Timestamp,
    double? UlcerIndex  // ulcer index
) : IReusable
{
    double IReusable.Value => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")] // v3.0.0
    public double? UI => UlcerIndex;
}
