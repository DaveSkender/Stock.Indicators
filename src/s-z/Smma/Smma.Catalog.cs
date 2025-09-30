namespace Skender.Stock.Indicators;

public static partial class Smma
{
    // Smoothed Moving Average Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Smoothed Moving Average")
            .WithId("SMMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToSmma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the SMMA calculation", isRequired: false, defaultValue: 20, minimum: 1, maximum: 250)
            .AddResult("Smma", "SMMA", ResultType.Default, isReusable: true)
            .Build();

    // Smoothed Moving Average Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // SMMA Stream Listing
    internal static readonly IndicatorListing StreamListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Stream)
            .Build();

    // SMMA Buffer Listing
    internal static readonly IndicatorListing BufferListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Buffer)
            .Build();
}
