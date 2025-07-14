namespace Skender.Stock.Indicators;

public static partial class HtTrendline
{
    // HTL Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Hilbert Transform Instantaneous Trendline")
            .WithId("HTL")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToHtTrendline")
            .AddResult("DcPeriods", "Dominant Cycle Periods", ResultType.Default, isDefault: false)
            .AddResult("Trendline", "Trendline", ResultType.Default, isDefault: true)
            .AddResult("SmoothPrice", "Smooth Price", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for HTL.
    // No BufferListing for HTL.
}
