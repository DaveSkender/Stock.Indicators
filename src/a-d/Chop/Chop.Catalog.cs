namespace Skender.Stock.Indicators;

public static partial class Chop
{
    // CHOP Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Choppiness Index")
            .WithId("CHOP")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToChop")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Chop", "CHOP", ResultType.Default, isReusable: true)
            .Build();

    // CHOP Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for CHOP.
    // No BufferListing for CHOP.
}
