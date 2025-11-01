namespace Skender.Stock.Indicators;

public static partial class WilliamsR
{
    /// <summary>
    /// Williams %R Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Williams %R")
            .WithId("WILLR")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToWilliamsR")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the Williams %R calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("WilliamsR", "Williams %R", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// Williams %R Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// Williams %R Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// Williams %R Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
