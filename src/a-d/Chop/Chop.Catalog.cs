namespace Skender.Stock.Indicators;

public static partial class Chop
{
    /// <summary>
    /// CHOP Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Choppiness Index")
            .WithId("CHOP")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToChop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Chop", "CHOP", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// CHOP Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// CHOP Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // No BufferListing for CHOP.
}
