namespace Skender.Stock.Indicators;

public static partial class FisherTransform
{
    // FISHER Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Ehlers Fisher Transform")
            .WithId("FISHER")
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToFisherTransform")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250)
            .AddResult("Fisher", "Fisher", ResultType.Default, isReusable: true)
            .AddResult("Trigger", "Trigger", ResultType.Default)
            .Build();

    // FISHER Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // FISHER Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // FISHER Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
