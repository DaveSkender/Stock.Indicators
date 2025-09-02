namespace Skender.Stock.Indicators;

public static partial class Obv
{
    // OBV Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new CatalogListingBuilder()
            .WithName("On-Balance Volume")
            .WithId("OBV")
            .WithStyle(Style.Series)
            .WithCategory(Category.VolumeBased)
            .WithMethodName("ToObv")
            .AddResult("Obv", "OBV", ResultType.Default, isReusable: true)
            .Build();

    // No StreamListing for OBV.
    // No BufferListing for OBV.
}
