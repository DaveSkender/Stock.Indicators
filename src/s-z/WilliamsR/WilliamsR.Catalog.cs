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

    // No StreamListing for Williams %R.
    // No BufferListing for Williams %R.
}
