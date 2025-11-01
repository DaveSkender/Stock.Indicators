namespace Skender.Stock.Indicators;

public static partial class Fcb
{
    /// <summary>
    /// FCB Common Base Listing
    /// </summary>
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

    /// <summary>
    /// FCB Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// FCB Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// FCB Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
