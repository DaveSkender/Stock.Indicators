namespace Skender.Stock.Indicators;

public static partial class Mfi
{
    // MFI Series Listing
    public static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Money Flow Index (MFI)") // From catalog.bak.json
            .WithId("MFI") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the MFI calculation", defaultValue: 14, minimum: 1, maximum: 250) // From catalog.bak.json
            .AddResult("Mfi", "MFI", ResultType.Default, isDefault: true) // From MfiResult model
            .Build();

    // No StreamListing for MFI.
    // No BufferListing for MFI.
}
