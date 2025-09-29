namespace Skender.Stock.Indicators;

public static partial class Mfi
{
    // MFI Common Base Listing
    internal static readonly IndicatorListing CommonListing =
        new CatalogListingBuilder()
            .WithName("Money Flow Index (MFI)")
            .WithId("MFI")
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToMfi")
            .AddParameter<int>("lookbackPeriods", "Lookback Periods", description: "Number of periods for the MFI calculation", defaultValue: 14, minimum: 1, maximum: 250)
            .AddResult("Mfi", "MFI", ResultType.Default, isReusable: true)
            .Build();

    // MFI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder(CommonListing)
            .WithStyle(Style.Series)
            .Build();

    // No StreamListing for MFI.
    // No BufferListing for MFI.
}
