namespace Skender.Stock.Indicators;

public static partial class Tr
{
    // TR Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("True Range")
            .WithId("TR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToTr")
            .AddResult("Tr", "True Range", ResultType.Default, isReusable: true)
            .Build();

    // TR Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder()
            .WithName("True Range")
            .WithId("TR")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToTr")
            .AddResult("Tr", "True Range", ResultType.Default, isReusable: true)
            .Build();
}
