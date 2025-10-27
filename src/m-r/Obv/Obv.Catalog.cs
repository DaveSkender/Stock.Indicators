namespace Skender.Stock.Indicators;

public static partial class Obv
{
    /// <summary>
    /// OBV Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("On-Balance Volume")
            .WithId("OBV")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToObv")
            .AddResult("Obv", "OBV", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// OBV Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// OBV Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// OBV Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
