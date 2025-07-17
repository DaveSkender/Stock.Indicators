namespace Skender.Stock.Indicators;

public static partial class Stoch
{
    // Stochastic Oscillator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Stochastic Oscillator")
            .WithId("STOCH")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToStoch")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the stochastic calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 3, minimum: 1, maximum: 250)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "%K", ResultType.Default, isReusable: true)
            .AddResult("Signal", "%D", ResultType.Default)
            .Build();
}
