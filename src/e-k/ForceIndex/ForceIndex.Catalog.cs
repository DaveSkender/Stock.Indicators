namespace Skender.Stock.Indicators;

public static partial class ForceIndex
{
    // FORCE Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Force Index") // From catalog.bak.json
            .WithId("FORCE") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .WithMethodName("ToForceIndex")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", defaultValue: 2, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("ForceIndex", "Force Index", ResultType.Default, isDefault: true) // From ForceIndexResult model
            .Build();

    // No StreamListing for FORCE.
    // No BufferListing for FORCE.
}
