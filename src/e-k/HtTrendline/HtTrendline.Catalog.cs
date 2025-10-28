namespace Skender.Stock.Indicators;

public static partial class HtTrendline
{
    /// <summary>
    /// HTL Common Base Listing
    /// </summary>
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

    /// <summary>
    /// HTL Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for HTL.

    /// <summary>
    /// HTL Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
