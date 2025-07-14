namespace Skender.Stock.Indicators;

public static partial class BollingerBands
{
    // BB Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Bollinger Bands®")
            .WithId("BB")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceChannel)
            .WithMethodName("ToBollingerBands")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 20, minimum: 2, maximum: 250)
            .AddParameter<double>("standardDeviations", "Standard Deviations", defaultValue: 2.0, minimum: 0.01, maximum: 10.0)
            .AddResult("Sma", "Centerline (SMA)", ResultType.Default, isDefault: false)
            .AddResult("UpperBand", "Upper Band", ResultType.Default, isDefault: false)
            .AddResult("LowerBand", "Lower Band", ResultType.Default, isDefault: false)
            .AddResult("PercentB", "%B", ResultType.Default, isDefault: true)
            .AddResult("ZScore", "Z-Score", ResultType.Default, isDefault: false)
            .AddResult("Width", "Width", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for BB.
    // No BufferListing for BB.
}
