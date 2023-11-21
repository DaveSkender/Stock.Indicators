namespace Skender.Stock.Indicators;

[Serializable]
public sealed class UlcerIndexResult : ResultBase, IReusableResult
{
    public UlcerIndexResult(DateTime date)
    {
        Date = date;
    }

    public double? UlcerIndex { get; set; } // ulcer index

    double IReusableResult.Value => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")]
    public double? UI => UlcerIndex;
}
