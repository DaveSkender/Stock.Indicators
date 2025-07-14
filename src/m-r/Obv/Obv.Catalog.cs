namespace Skender.Stock.Indicators;

public static partial class Obv
{
    // OBV Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("On-Balance Volume")
            .WithId("OBV")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToObv")
            .AddResult("Obv", "OBV", ResultType.Default, isDefault: true)
            .Build();

    // No StreamListing for OBV.
    // No BufferListing for OBV.
}
