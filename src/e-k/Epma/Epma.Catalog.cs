namespace Skender.Stock.Indicators;

public static partial class Epma
{
    // EPMA Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Endpoint Moving Average")
            .WithId("EPMA")
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToEpma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250)
            .AddResult("Epma", "EPMA", ResultType.Default, isReusable: true)
            .Build();

    // EPMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for EPMA.
    // No BufferListing for EPMA.
}
