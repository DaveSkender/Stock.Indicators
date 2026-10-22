namespace FacioQuo.Stock.Indicators;

public static partial class ForceIndex
{
    /// <summary>
    /// FORCE Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Force Index")
            .WithId("FORCE")
            .WithCategory(Category.VolumeBased)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 13, minimum: 1, maximum: 250)
            .AddResult(nameof(ForceIndexResult.ForceIndex), "Force Index", "ForceIndex", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// FORCE Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName(typeof(ForceIndex), "ToForceIndex")
            .Build();

    /// <summary>
    /// FORCE Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .WithMethodName(typeof(ForceIndex), "ToForceIndexList")
            .Build();

    /// <summary>
    /// FORCE Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .WithMethodName(typeof(ForceIndex), "ToForceIndexHub")
            .Build();
}
