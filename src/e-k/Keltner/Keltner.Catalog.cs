namespace Skender.Stock.Indicators;

public static partial class Keltner
{
    // KELTNER Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Keltner Channels")
            .WithId("KELTNER")
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToKeltner")
            .AddParameter<int>("emaPeriods", "EMA Periods", defaultValue: 20, minimum: 2, maximum: 250)
            .AddParameter<double>("multiplier", "Multiplier", defaultValue: 2.0, minimum: 0.01, maximum: 10.0)
            .AddParameter<int>("atrPeriods", "ATR Periods", defaultValue: 10, minimum: 2, maximum: 250)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("Centerline", "Centerline", ResultType.Default, isReusable: true)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .AddResult("Width", "Width", ResultType.Default)
            .Build();

    // KELTNER Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // KELTNER Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();

    // No StreamListing for KELTNER.
}
