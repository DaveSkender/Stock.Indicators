namespace Skender.Stock.Indicators;

public static partial class Rsi
{
    // RSI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Relative Strength Index")
            .WithId("RSI")
            .WithStyle(Style.Series)
            .WithCategory(Category.Oscillator)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the RSI calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Rsi", "RSI", ResultType.Default, isDefault: true)
            .Build();
}