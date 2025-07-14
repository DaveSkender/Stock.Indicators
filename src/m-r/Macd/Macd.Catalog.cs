namespace Skender.Stock.Indicators;

public static partial class Macd
{
    // MACD Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Moving Average Convergence/Divergence") // From catalog.bak.json
            .WithId("MACD") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend) // From catalog.bak.json Category: "PriceTrend"
            .WithMethodName("ToMacd")
            .AddParameter<int>("fastPeriods", "Fast Periods", description: "Number of periods for the fast EMA", isRequired: false, defaultValue: 12, minimum: 1, maximum: 200) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", description: "Number of periods for the slow EMA", isRequired: false, defaultValue: 26, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 9, minimum: 1, maximum: 50) // From catalog.bak.json
            .AddResult("Macd", "MACD", ResultType.Default, isDefault: true) // From MacdResult model
            .AddResult("Signal", "Signal", ResultType.Default, isDefault: false) // From MacdResult model
            .AddResult("Histogram", "Histogram", ResultType.Bar, isDefault: false) // From MacdResult model
            .Build();

    // No StreamListing for MACD.
    // No BufferListing for MACD.
}
