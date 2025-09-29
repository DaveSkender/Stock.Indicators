namespace Skender.Stock.Indicators;

public static partial class Awesome
{
    // AWESOME Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Awesome Oscillator")
            .WithId("AWESOME")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToAwesome")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 5, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 34, minimum: 1, maximum: 250)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .AddResult("Normalized", "Normalized", ResultType.Default)
            .Build();

    // AWESOME Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for AWESOME.
    // No BufferListing for AWESOME.
}
