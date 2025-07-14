namespace Skender.Stock.Indicators;

public static partial class Pmo
{
    // Price Momentum Oscillator Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Price Momentum Oscillator")
            .WithId("PMO")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToPmo")
            .AddParameter<int>("timePeriods", "Time Periods", description: "Number of periods for the time frame", isRequired: false, defaultValue: 35, minimum: 1, maximum: 250)
            .AddParameter<int>("smoothingPeriods", "Smoothing Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 20, minimum: 1, maximum: 100)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 10, minimum: 1, maximum: 50)
            .AddResult("Pmo", "PMO", ResultType.Default, isDefault: true)
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false)
            .Build();
}
