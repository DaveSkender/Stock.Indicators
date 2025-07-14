namespace Skender.Stock.Indicators;

public static partial class SmaAnalysis
{
    // SMA Analysis Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Simple Moving Average Analysis")
            .WithId("SMA-ANALYSIS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToSmaAnalysis")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA analysis", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .AddResult("Mad", "Mean Absolute Deviation", ResultType.Default, isDefault: false)
            .AddResult("Mse", "Mean Square Error", ResultType.Default, isDefault: false)
            .AddResult("Mape", "Mean Absolute Percentage Error", ResultType.Default, isDefault: false)
            .Build();
}
