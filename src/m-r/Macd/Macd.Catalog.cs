namespace Skender.Stock.Indicators;

public static partial class Macd
{
    // MACD Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Moving Average Convergence/Divergence")
            .WithId("MACD")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToMacd")
            .AddParameter<int>("fastPeriods", "Fast Periods", description: "Number of periods for the fast EMA", isRequired: false, defaultValue: 12, minimum: 1, maximum: 200)
            .AddParameter<int>("slowPeriods", "Slow Periods", description: "Number of periods for the slow EMA", isRequired: false, defaultValue: 26, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 9, minimum: 1, maximum: 50)
            .AddResult("Macd", "MACD", ResultType.Default, isReusable: true)
            .AddResult("Signal", "Signal", ResultType.Default)
            .AddResult("Histogram", "Histogram", ResultType.Bar)
            .Build();

    // No StreamListing for MACD.
    // No BufferListing for MACD.
}
