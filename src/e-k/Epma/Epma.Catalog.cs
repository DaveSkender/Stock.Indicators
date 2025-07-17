namespace Skender.Stock.Indicators;

public static partial class Epma
{
    // EPMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Endpoint Moving Average")
            .WithId("EPMA")
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage)
            .WithMethodName("ToEpma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250)
            .AddResult("Epma", "EPMA", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for EPMA.
    // No BufferListing for EPMA.
}
