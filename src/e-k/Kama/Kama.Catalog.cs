namespace Skender.Stock.Indicators;

public static partial class Kama
{
    // KAMA Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Kaufman's Adaptive Moving Average") // From catalog.bak.json
            .WithId("KAMA") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.MovingAverage) // From catalog.bak.json Category: "MovingAverage"
            .AddParameter<int>("erPeriods", "ER Periods", defaultValue: 10, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddParameter<int>("fastPeriods", "Fast Periods", defaultValue: 2, minimum: 1, maximum: 50) // From catalog.bak.json
            .AddParameter<int>("slowPeriods", "Slow Periods", defaultValue: 30, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Er", "ER", ResultType.Default, isDefault: false) // From KamaResult model
            .AddResult("Kama", "KAMA", ResultType.Default, isDefault: true) // From KamaResult model
            .Build();

    // No StreamListing for KAMA.
    // No BufferListing for KAMA.
}
