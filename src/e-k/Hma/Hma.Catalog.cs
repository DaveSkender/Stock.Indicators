namespace Skender.Stock.Indicators;

public static partial class Hma
{
    // HMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Hull Moving Average") // From catalog.bak.json
            .WithId("HMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 14, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("Hma", "HMA", ResultType.Default, isDefault: true) // From HmaResult model
            .Build();

    // No StreamListing for HMA.
    // No BufferListing for HMA.
}
