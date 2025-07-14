namespace Skender.Stock.Indicators;

public static partial class Smi
{
    // Stochastic Momentum Index Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Stochastic Momentum Index")
            .WithId("SMI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToSmi")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMI calculation", isRequired: false, defaultValue: 13, minimum: 1, maximum: 300)
            .AddParameter<int>("firstSmoothPeriods", "First Smooth Periods", description: "Number of periods for the first smoothing", isRequired: false, defaultValue: 25, minimum: 1, maximum: 300)
            .AddParameter<int>("secondSmoothPeriods", "Second Smooth Periods", description: "Number of periods for the second smoothing", isRequired: false, defaultValue: 2, minimum: 1, maximum: 50)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 3, minimum: 1, maximum: 50)
            .AddResult("Smi", "SMI", ResultType.Default, isDefault: true)
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false)
            .Build();
}
