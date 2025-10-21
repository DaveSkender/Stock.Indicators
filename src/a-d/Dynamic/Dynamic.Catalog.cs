namespace Skender.Stock.Indicators;

public static partial class MgDynamic
{
    // Dynamic Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("McGinley Dynamic")
            .WithId("DYNAMIC")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToDynamic")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the McGinley Dynamic calculation", isRequired: true, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<double>("kFactor", "K Factor", description: "Smoothing factor for the calculation", isRequired: false, defaultValue: 0.6, minimum: 0.1, maximum: 2.0)
            .AddResult("Dynamic", "McGinley Dynamic", ResultType.Default, isReusable: true)
            .Build();

    // Dynamic Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // Dynamic Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // Dynamic Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
