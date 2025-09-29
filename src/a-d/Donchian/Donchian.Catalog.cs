namespace Skender.Stock.Indicators;

public static partial class Donchian
{
    // DONCHIAN Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Donchian Channels")
            .WithId("DONCHIAN")
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToDonchian")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("Centerline", "Centerline", ResultType.Default, isReusable: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .AddResult("Width", "Width", ResultType.Default)
            .Build();

    // DONCHIAN Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for DONCHIAN.
    // No BufferListing for DONCHIAN.
}
