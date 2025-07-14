namespace Skender.Stock.Indicators;

public static partial class Mfi
{
    // MFI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("Money Flow Index (MFI)")
            .WithId("MFI")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToMfi")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the MFI calculation", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Mfi", "MFI", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for MFI.
    // No BufferListing for MFI.
}
