namespace Skender.Stock.Indicators;

public static partial class Slope
{
    /// <summary>
    /// Slope Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Slope")
            .WithId("SLOPE")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToSlope")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the slope calculation", isRequired: false, defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Slope", "Slope", ResultType.Default, isReusable: true)
            .AddResult("Intercept", "Intercept", ResultType.Default)
            .AddResult("StdDev", "Standard Deviation", ResultType.Default)
            .AddResult("RSquared", "R-Squared", ResultType.Default)
            .Build();

    /// <summary>
    /// Slope Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// Slope Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    /// <summary>
    /// Slope Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
