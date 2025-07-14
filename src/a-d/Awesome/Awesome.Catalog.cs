namespace Skender.Stock.Indicators;

public static partial class Awesome
{
    // AWESOME Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Awesome Oscillator")
            .WithId("AWESOME")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToAwesome")
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 5, minimum: 1, maximum: 100)
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 34, minimum: 1, maximum: 250)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true)
            .AddResult("Normalized", "Normalized", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for AWESOME.
    // No BufferListing for AWESOME.
}
