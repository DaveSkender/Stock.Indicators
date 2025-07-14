namespace Skender.Stock.Indicators;

public static partial class Kvo
{
    // KVO Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Klinger Volume Oscillator")
            .WithId("KVO")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToKvo")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 34, minimum: 1, maximum: 200)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 55, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", defaultValue: 13, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true)
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for KVO.
    // No BufferListing for KVO.
}
