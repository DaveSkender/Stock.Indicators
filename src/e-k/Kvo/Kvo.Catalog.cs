namespace Skender.Stock.Indicators;

public static partial class Kvo
{
    // KVO Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Klinger Volume Oscillator")
            .WithId("KVO")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToKvo")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 34, minimum: 1, maximum: 200)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 55, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", defaultValue: 13, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .AddResult("Signal", "Signal", ResultType.Default)
            .Build();

    // KVO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // KVO Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // KVO Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
