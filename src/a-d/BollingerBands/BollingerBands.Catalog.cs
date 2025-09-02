namespace Skender.Stock.Indicators;

public static partial class BollingerBands
{
    // BB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Bollinger Bands®")
            .WithId("BB")
            .WithStyle(Style.Series)
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

    // No StreamListing for BB.
    // No BufferListing for BB.
}
