namespace Skender.Stock.Indicators;

public static partial class Tr
{
    // TR Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("True Range")
            .WithId("TR")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: true)
            .Build();

    // TR Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("True Range")
            .WithId("TR")
            .WithStyle(Style.Stream)
            .WithCategory(Category.PriceCharacteristic)
            .AddResult("Tr", "True Range", ResultType.Default, isDefault: true)
            .Build();
}