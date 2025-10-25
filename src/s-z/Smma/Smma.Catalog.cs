namespace Skender.Stock.Indicators;

public static partial class Smma
{
    /// <summary>
    /// Smoothed Moving Average Common Base Listing
    /// </summary>
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Smoothed Moving Average")
            .WithId("SMMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToSmma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMMA calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Smma", "SMMA", ResultType.Default, isReusable: true)
            .Build();

    /// <summary>
    /// Smoothed Moving Average Series Listing
    /// </summary>
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    /// <summary>
    /// SMMA Stream Listing
    /// </summary>
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    /// <summary>
    /// SMMA Buffer Listing
    /// </summary>
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
