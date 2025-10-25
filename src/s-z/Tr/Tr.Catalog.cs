namespace Skender.Stock.Indicators;

public static partial class Tr
{
    // TR Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("True Range")
            .WithId("TR")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToTr")
            .AddResult("Tr", "True Range", ResultType.Default, isReusable: true)
            .Build();

    // TR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // TR Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // TR Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
