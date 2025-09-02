namespace Skender.Stock.Indicators;

public static partial class StochRsi
{
    // Stochastic RSI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Stochastic RSI")
            .WithId("STOCH-RSI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToStochRsi")
            .AddParameter<int>("rsiPeriods", "RSI Periods", description: "Number of periods for the RSI calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<int>("stochPeriods", "Stochastic Periods", description: "Number of periods for the Stochastic calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddParameter<int>("signalPeriods", "Signal Periods", description: "Number of periods for the signal line", isRequired: false, defaultValue: 3, minimum: 1, maximum: 50)
            .AddParameter<int>("smoothPeriods", "Smooth Periods", description: "Number of periods for smoothing", isRequired: false, defaultValue: 1, minimum: 1, maximum: 50)
            .AddResult("StochRsi", "%K", ResultType.Default, isReusable: true)
            .AddResult("Signal", "%D", ResultType.Default)
            .Build();
}
