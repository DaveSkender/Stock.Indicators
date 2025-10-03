namespace Skender.Stock.Indicators;

public static partial class WilliamsR
{
    // Williams %R Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Williams %R")
            .WithId("WILLR")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToWilliamsR")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Williams %R calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("WilliamsR", "Williams %R", ResultType.Default, isReusable: true)
            .Build();

    // Williams %R Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // Williams %R Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // Williams %R Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
