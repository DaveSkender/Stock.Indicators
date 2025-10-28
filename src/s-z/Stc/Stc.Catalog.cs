namespace Skender.Stock.Indicators;

public static partial class Stc
{
    /// <summary>
    /// Schaff Trend Cycle Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Schaff Trend Cycle")
            .WithId("STC")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToStc")
            .AddParameter<int>("cyclePeriods", "Cycle Periods", description: "Number of periods for the cycle calculation", isRequired: false, defaultValue: 10, minimum: 1, maximum: 250)
            .AddParameter<int>("fastPeriods", "Fast Periods", description: "Number of periods for the fast MA", isRequired: false, defaultValue: 23, minimum: 1, maximum: 250)
            .AddParameter<int>("slowPeriods", "Slow Periods", description: "Number of periods for the slow MA", isRequired: false, defaultValue: 50, minimum: 1, maximum: 250)
            .AddResult("Stc", "STC", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// Schaff Trend Cycle Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// Schaff Trend Cycle Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // No StreamListing for STC.
}
