namespace Skender.Stock.Indicators;

public static partial class Adl
{
    /// <summary>
    /// ADL Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Accumulation Distribution Line (ADL)")
            .WithId("ADL")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToAdl")
            .AddResult("Adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// ADL Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// ADL Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// ADL Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
