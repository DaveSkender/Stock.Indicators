namespace Skender.Stock.Indicators;

public static partial class Obv
{
    // OBV Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("On-Balance Volume")
            .WithId("OBV")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToObv")
            .AddResult("Obv", "OBV", ResultType.Default, isReusable: true)
            .Build();

    // OBV Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // OBV Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // OBV Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
