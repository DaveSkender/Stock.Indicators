namespace Skender.Stock.Indicators;

public static partial class Sma
{
    // SMA Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .Build();

    // SMA Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .Build();

    // SMA Analysis Series Listing
    public static readonly IndicatorListing AnalysisListing =
        new IndicatorListingBuilder()
            .WithName("Simple Moving Average Analysis")
            .WithId("SMA-ANALYSIS")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA analysis", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .AddResult("Mad", "Mean Absolute Deviation", ResultType.Default, isDefault: false)
            .AddResult("Mse", "Mean Square Error", ResultType.Default, isDefault: false)
            .AddResult("Mape", "Mean Absolute Percentage Error", ResultType.Default, isDefault: false)
            .Build();
}