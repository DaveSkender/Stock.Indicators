namespace Skender.Stock.Indicators;

public static partial class Doji
{
    // DOJI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Doji")
            .WithId("DOJI")
            .WithStyle(Style.Series)
            .WithCategory(Category.CandlestickPattern)
            .WithMethodName("ToDoji")
            .AddParameter<double>("maxPriceChangePercent", "Max Price Change %", defaultValue: 0.1, minimum: 0.0, maximum: 0.5)
            .AddResult("Match", "Match", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for DOJI.
    // No BufferListing for DOJI.
}
