namespace Skender.Stock.Indicators;

public static partial class Sma
{
    // SMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToSma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isReusable: true)
            .Build();

    // SMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithStyle(Style.Stream)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToSma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isReusable: true)
            .Build();
}
