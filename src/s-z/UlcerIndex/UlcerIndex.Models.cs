namespace Skender.Stock.Indicators;

public sealed record class UlcerIndexResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? UlcerIndex { get; set; } // ulcer index

    double IReusableResult.Value => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")]
    public double? UI => UlcerIndex;
}
