namespace Skender.Stock.Indicators;

public static partial class Rsi
{
    // RSI Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Relative Strength Index")
            .WithId("RSI")
            .WithCategory(Category.Oscillator)
            .WithMethodName("ToRsi")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the RSI calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Rsi", "RSI", ResultType.Default, isReusable: true)
            .Build();

    // RSI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // RSI Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // RSI Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
