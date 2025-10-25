namespace Skender.Stock.Indicators;

public static partial class HtTrendline
{
    // HTL Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Hilbert Transform Instantaneous Trendline")
            .WithId("HTL")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToHtTrendline")
            .AddResult("DcPeriods", "Dominant Cycle Periods", ResultType.Default)
            .AddResult("Trendline", "Trendline", ResultType.Default, isReusable: true)
            .AddResult("SmoothPrice", "Smooth Price", ResultType.Default)
            .Build();

    // HTL Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for HTL.
    // No BufferListing for HTL.
}
