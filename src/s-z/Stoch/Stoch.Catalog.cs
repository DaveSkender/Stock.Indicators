namespace Skender.Stock.Indicators;

public static partial class Stoch
{
    // Stochastic Oscillator Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Stochastic Oscillator")
            .WithId("STOCH")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToStoch")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the stochastic calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 3, minimum: 1, maximum: 250)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "%K", ResultType.Default, isReusable: true)
            .AddResult("Signal", "%D", ResultType.Default)
            .Build();

    // Stochastic Oscillator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for Stochastic Oscillator.
    // No BufferListing for Stochastic Oscillator.
}
