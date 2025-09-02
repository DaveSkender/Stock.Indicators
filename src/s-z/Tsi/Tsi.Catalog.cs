namespace Skender.Stock.Indicators;

public static partial class Tsi
{
    // True Strength Index Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("True Strength Index")
            .WithId("TSI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToTsi")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the TSI calculation", isRequired: false, defaultValue: 25, minimum: 1, maximum: 250)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 13, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 7, minimum: 1, maximum: 50)
            .AddResult("Tsi", "TSI", ResultType.Default, isReusable: true)
            .AddResult("Signal", "Signal", ResultType.Default)
            .Build();
}
