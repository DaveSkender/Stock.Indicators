namespace Skender.Stock.Indicators;

public static partial class StdDev
{
    // Standard Deviation Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("Standard Deviation")
            .WithId("STDEV")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic)
            .WithMethodName("ToStdDev")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the standard deviation calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("StdDev", "Standard Deviation", ResultType.Default, isReusable: true)
            .Build();
}
