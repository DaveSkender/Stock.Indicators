namespace Skender.Stock.Indicators;

public static partial class Doji
{
    /// <summary>
    /// DOJI Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Doji")
            .WithId("DOJI")
            .WithCategory(Category.CandlestickPattern)
            .AddParameter<double>("maxPriceChangePercent", "Max Price Change %", defaultValue: 0.1, minimum: 0.0, maximum: 0.5)
            .AddResult("Match", "Match", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// DOJI Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToDoji")
            .Build();

    // No StreamListing for DOJI.
    // No BufferListing for DOJI.
}
