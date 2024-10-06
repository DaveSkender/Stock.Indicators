namespace Skender.Stock.Indicators;

public record UlcerIndexResult
(
    DateTime Timestamp,
    double? UlcerIndex
) : Reusable(Timestamp)
{
    public override double Value => UlcerIndex.Null2NaN();

    [Obsolete("Rename UI to UlcerIndex")] // v3.0.0
    public double? UI => UlcerIndex;
}
