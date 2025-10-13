namespace Skender.Stock.Indicators;

public static partial class Vortex
{
    // Vortex Indicator Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Vortex Indicator")
            .WithId("VORTEX")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToVortex")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Vortex calculation", isRequired: false, defaultValue: 14, minimum: 2, maximum: 100)
            .AddResult("Pvi", "VI+", ResultType.Default, isReusable: true)
            .AddResult("Nvi", "VI-", ResultType.Default)
            .Build();

    // Vortex Indicator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // Vortex Indicator Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // No StreamListing for Vortex Indicator.
}
