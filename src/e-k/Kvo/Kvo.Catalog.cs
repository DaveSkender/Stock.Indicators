namespace Skender.Stock.Indicators;

public static partial class Kvo
{
    /// <summary>
    /// KVO Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Klinger Volume Oscillator")
            .WithId("KVO")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToKvo")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 34, minimum: 3, maximum: 200)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 55, minimum: 4, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", defaultValue: 13, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .AddResult("Signal", "Signal", ResultType.Default)
            .Build();

    /// <summary>
    /// KVO Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// KVO Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    /// <summary>
    /// KVO Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithMethodName("ToKvoHub")
            .WithStyle(Style.Stream)
            .Build();
}
