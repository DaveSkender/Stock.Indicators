namespace Skender.Stock.Indicators;

public record struct UlcerIndexResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? UlcerIndex { get; set; } // ulcer index

    readonly double IReusable.Value
        => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")]
    public readonly double? UI => UlcerIndex;
}
