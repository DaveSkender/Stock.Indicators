namespace Skender.Stock.Indicators;

public static partial class Aroon
{
    // AROON Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Aroon Up/Down")
            .WithId("AROON")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAroon")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 25, minimum: 1, maximum: 250)
            .AddResult("AroonUp", "Aroon Up", ResultType.Default, isDefault: false)
            .AddResult("AroonDown", "Aroon Down", ResultType.Default, isDefault: false)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for AROON.
    // No BufferListing for AROON.
}
