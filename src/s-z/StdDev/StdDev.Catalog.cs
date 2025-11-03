namespace Skender.Stock.Indicators;

public static partial class StdDev
{
    /// <summary>
    /// Standard Deviation Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Standard Deviation")
            .WithId("STDEV")
            .WithCategory(Category.PriceCharacteristic)
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the standard deviation calculation", isRequired: false, defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("StdDev", "Standard Deviation", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// Standard Deviation Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .WithMethodName("ToStdDev")
            .Build();

    // No StreamListing for Standard Deviation.
    // No BufferListing for Standard Deviation.
}
