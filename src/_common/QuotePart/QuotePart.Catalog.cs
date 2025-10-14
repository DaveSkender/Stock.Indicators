namespace Skender.Stock.Indicators;

public static partial class QuoteParts
{
    // QUOTEPART Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Quote Part")
            .WithId("QUOTEPART")
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToQuotePart")
            .AddEnumParameter<CandlePart>("candlePart", "Candle Part", defaultValue: CandlePart.Close)
            .AddResult("Value", "Value", ResultType.Default, isReusable: true)
            .Build();

    // QUOTEPART Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // QUOTEPART Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // QUOTEPART Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
