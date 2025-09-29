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

    // No StreamListing for FORCE.
    // No BufferListing for FORCE.
}
