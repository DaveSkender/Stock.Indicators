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
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .Build();

    // SMA Stream Listing
    public static readonly IndicatorListing StreamListing =
        new IndicatorListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 2, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isDefault: true)
            .Build();


}
