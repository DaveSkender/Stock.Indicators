namespace Skender.Stock.Indicators;

public static partial class FisherTransform
{
    // FISHER Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Ehlers Fisher Transform") // From catalog.bak.json
            .WithId("FISHER") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform) // From catalog.bak.json Category: "PriceTransform"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 10, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Fisher", "Fisher", ResultType.Default, isDefault: true) // From FisherTransformResult model
            .AddResult("Trigger", "Trigger", ResultType.Default, isDefault: false) // From FisherTransformResult model
            .Build();

    // No StreamListing for FISHER.
    // No BufferListing for FISHER.
}
