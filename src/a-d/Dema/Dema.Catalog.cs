namespace Skender.Stock.Indicators;

public static partial class Dema
{
    // DEMA Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Double Exponential Moving Average")
            .WithId("DEMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToDema")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the DEMA calculation", isRequired: false, defaultValue: 14, minimum: 2, maximum: 250)
            .AddResult("Dema", "DEMA", ResultType.Default, isReusable: true)
            .Build();

    // DEMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // DEMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // DEMA Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
