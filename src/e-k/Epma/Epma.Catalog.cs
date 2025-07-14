namespace Skender.Stock.Indicators;

public static partial class Epma
{
    // EPMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Endpoint Moving Average") // From catalog.bak.json
            .WithId("EPMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .WithMethodName("ToEpma")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Epma", "EPMA", ResultType.Default, isDefault: true) // From EpmaResult model
            .Build();

    // No StreamListing for EPMA.
    // No BufferListing for EPMA.
}
