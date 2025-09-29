namespace Skender.Stock.Indicators;

public static partial class Fcb
{
    // FCB Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Fractal Chaos Bands")
            .WithId("FCB")
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToFcb")
            .AddParameter<int>("windowSpan", "Window Span", defaultValue: 2, minimum: 2, maximum: 30)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isReusable: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .Build();

    // FCB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for FCB.
    // No BufferListing for FCB.
}
