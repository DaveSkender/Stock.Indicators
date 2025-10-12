namespace Skender.Stock.Indicators;

public static partial class HeikinAshi
{
    // HEIKINASHI Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("HeikinAshi")
            .WithId("HEIKINASHI")
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToHeikinAshi")
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .Build();

    // HEIKINASHI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // HEIKINASHI Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // No StreamListing for HEIKINASHI.
}
