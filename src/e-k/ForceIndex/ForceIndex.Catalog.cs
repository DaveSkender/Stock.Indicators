namespace Skender.Stock.Indicators;

public static partial class ForceIndex
{
    // FORCE Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Force Index")
            .WithId("FORCE")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToForceIndex")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 2, minimum: 1, maximum: 250)
            .AddResult("ForceIndex", "Force Index", ResultType.Default, isReusable: true)
            .Build();

    // FORCE Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // FORCE Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // FORCE Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
