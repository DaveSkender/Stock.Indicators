namespace Skender.Stock.Indicators;

public static partial class Aroon
{
    // AROON Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Aroon Up/Down")
            .WithId("AROON")
            .WithCategory(Category.PriceTrend)
            .WithMethodName("ToAroon")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 25, minimum: 1, maximum: 250)
            .AddResult("AroonUp", "Aroon Up", ResultType.Default)
            .AddResult("AroonDown", "Aroon Down", ResultType.Default)
            .AddResult("Oscillator", "Oscillator", ResultType.Default, isReusable: true)
            .Build();

    // AROON Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for AROON.
    // No BufferListing for AROON.
}
