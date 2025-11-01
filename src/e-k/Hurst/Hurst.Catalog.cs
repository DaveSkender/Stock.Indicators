namespace Skender.Stock.Indicators;

public static partial class Hurst
{
    /// <summary>
    /// HURST Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Hurst Exponent")
            .WithId("HURST")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToHurst")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 100, minimum: 2, maximum: 250)
            .AddResult("HurstExponent", "Hurst Exponent", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// HURST Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// HURST Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    /// <summary>
    /// HURST Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
