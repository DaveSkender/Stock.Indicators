namespace Skender.Stock.Indicators;

public static partial class Kvo
{
    // KVO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Klinger Volume Oscillator") // From catalog.bak.json
            .WithId("KVO") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 34, minimum: 1, maximum: 200) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 55, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("signalPeriods", "Signal Periods", defaultValue: 13, minimum: 1, maximum: 50) // From catalog.bak.json
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true) // From KvoResult model
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false) // From KvoResult model
            .Build();

    // No StreamListing for KVO.
    // No BufferListing for KVO.
}
