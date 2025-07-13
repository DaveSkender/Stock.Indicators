namespace Skender.Stock.Indicators;

public static partial class Stoch
{
    // Stochastic Oscillator Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Stochastic Oscillator")
            .WithId("STOCH")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the stochastic calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 3, minimum: 1, maximum: 250)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Oscillator", "%K", ResultType.Default, isDefault: true)
            .AddResult("Signal", "%D", ResultType.Default, isDefault: false)
            .Build();
}