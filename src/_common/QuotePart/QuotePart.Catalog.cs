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
            .WithMethodName("ToQuotePart")
            .AddEnumParameter<CandlePart>("candlePart", "Candle Part", defaultValue: CandlePart.Close)
            .AddResult("Value", "Value", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// QUOTEPART Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// QUOTEPART Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// QUOTEPART Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
