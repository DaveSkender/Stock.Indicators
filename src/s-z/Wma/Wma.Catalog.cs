namespace Skender.Stock.Indicators;

public static partial class Wma
{
    // Weighted Moving Average Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Weighted Moving Average")
            .WithId("WMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToWma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the WMA calculation", isRequired: true, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Wma", "WMA", ResultType.Default, isReusable: true)
            .Build();

    // Weighted Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // Weighted Moving Average Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // Weighted Moving Average Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
