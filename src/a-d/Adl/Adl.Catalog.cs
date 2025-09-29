namespace Skender.Stock.Indicators;

public static partial class Adl
{
    // ADL Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Accumulation Distribution Line (ADL)")
            .WithId("ADL")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToAdl")
            .AddResult("Adl", "Accumulation Distribution Line (ADL)", ResultType.Default, isReusable: true)
            .Build();

    // ADL Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // ADL Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // No BufferListing for ADL.
}
