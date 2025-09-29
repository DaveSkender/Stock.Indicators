namespace Skender.Stock.Indicators;

public static partial class Sma
{
    // SMA Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Simple Moving Average")
            .WithId("SMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToSma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMA calculation", isRequired: true, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Sma", "SMA", ResultType.Default, isReusable: true)
            .Build();

    // SMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // SMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();
}
