namespace Skender.Stock.Indicators;

public static partial class Fcb
{
    // FCB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Fractal Chaos Bands")
            .WithId("FCB")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToFcb")
            .AddParameter<int>("windowSpan", "Window Span", defaultValue: 2, minimum: 2, maximum: 30)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isReusable: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .Build();

    // No StreamListing for FCB.
    // No BufferListing for FCB.
}
