namespace Skender.Stock.Indicators;

public static partial class SmaAnalysis
{
    /// <summary>
    /// SMA Analysis Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Simple Moving Average Analysis")
            .WithId("SMA-ANALYSIS")
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToSmaAnalysis")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA analysis", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isReusable: true)
            .AddResult("Mad", "Mean Absolute Deviation", ResultType.Default)
            .AddResult("Mse", "Mean Square Error", ResultType.Default)
            .AddResult("Mape", "Mean Absolute Percentage Error", ResultType.Default)
            .Build();

    /// <summary>
    /// SMA Analysis Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for SMA Analysis.
    // No BufferListing for SMA Analysis.
}
