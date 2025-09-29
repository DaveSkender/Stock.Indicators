namespace Skender.Stock.Indicators;

public static partial class Pvo
{
    // Price Volume Oscillator Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Price Volume Oscillator")
            .WithId("PVO")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToPvo")
            .AddParameter<int>("fastPeriods", "Fast Periods", description: "Number of periods for the fast EMA", isRequired: false, defaultValue: 12, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", description: "Number of periods for the slow EMA", isRequired: false, defaultValue: 26, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 9, minimum: 1, maximum: 50)
            .AddResult("Pvo", "PVO", ResultType.Default, isReusable: true)
            .AddResult("Signal", "Signal", ResultType.Default)
            .AddResult("Histogram", "Histogram", ResultType.Default)
            .Build();

    // Price Volume Oscillator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for PVO.
    // No BufferListing for PVO.
}
