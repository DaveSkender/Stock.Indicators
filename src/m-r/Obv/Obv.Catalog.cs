namespace Skender.Stock.Indicators;

public static partial class Obv
{
    // OBV Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("On-Balance Volume") // From catalog.bak.json
            .WithId("OBV") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased) // From catalog.bak.json Category: "VolumeBased"
            .WithMethodName("ToObv")
                                                // No parameters for OBV in catalog.bak.json
            .AddResult("Obv", "OBV", ResultType.Default, isDefault: true) // From ObvResult model
            .Build();

    // No StreamListing for OBV.
    // No BufferListing for OBV.
}
