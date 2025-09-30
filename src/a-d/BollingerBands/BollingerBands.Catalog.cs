namespace Skender.Stock.Indicators;

public static partial class BollingerBands
{
    // BB Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Bollinger Bands®")
            .WithId("BB")
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToBollingerBands")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250)
            .AddParameter<double>("standardDeviations", "Standard Deviations", defaultValue: 2.0, minimum: 0.01, maximum: 10.0)
            .AddResult("Sma", "Centerline (SMA)", ResultType.Default)
            .AddResult("UpperBand", "Upper Band", ResultType.Default)
            .AddResult("LowerBand", "Lower Band", ResultType.Default)
            .AddResult("PercentB", "%B", ResultType.Default, isReusable: true)
            .AddResult("ZScore", "Z-Score", ResultType.Default)
            .AddResult("Width", "Width", ResultType.Default)
            .Build();

    // BB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // BB Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // BB Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
