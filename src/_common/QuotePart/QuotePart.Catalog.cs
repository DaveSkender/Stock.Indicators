namespace Skender.Stock.Indicators;

public static partial class QuoteParts
{
    /// <summary>
    /// QUOTEPART Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Quote Part")
            .WithId("QUOTEPART")
            .WithCategory(Category.PriceTransform)
            .AddEnumParameter<CandlePart>("candlePart", "Candle Part", defaultValue: CandlePart.Close)
            .AddResult("Value", "Value", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// QUOTEPART Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToQuotePart")
            .Build();

    /// <summary>
    /// QUOTEPART Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName("ToQuotePartHub")
            .Build();

    /// <summary>
    /// QUOTEPART Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName("ToQuotePartList")
            .Build();
}
