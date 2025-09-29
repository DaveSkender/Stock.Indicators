namespace Skender.Stock.Indicators;

public static partial class Kama
{
    // KAMA Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Kaufman's Adaptive Moving Average")
            .WithId("KAMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToKama")
            .AddParameter<int>("erPeriods", "ER Periods", defaultValue: 10, minimum: 2, maximum: 250)
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 2, minimum: 1, maximum: 50)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 30, minimum: 1, maximum: 250)
            .AddResult("Er", "ER", ResultType.Default)
            .AddResult("Kama", "KAMA", ResultType.Default, isReusable: true)
            .Build();

    // KAMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for KAMA.
    // No BufferListing for KAMA.
}
