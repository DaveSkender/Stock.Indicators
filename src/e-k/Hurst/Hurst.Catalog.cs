namespace Skender.Stock.Indicators;

public static partial class Hurst
{
    // HURST Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Hurst Exponent") // From catalog.bak.json
            .WithId("HURST") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceCharacteristic) // From catalog.bak.json Category: "PriceCharacteristic"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 100, minimum: 2, maximum: 250) // From catalog.bak.json
            .AddResult("HurstExponent", "Hurst Exponent", ResultType.Default, isDefault: true) // From HurstResult model
            .Build();

    // No StreamListing for HURST.
    // No BufferListing for HURST.
}
