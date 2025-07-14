namespace Skender.Stock.Indicators;

public static partial class HeikinAshi
{
    // HEIKINASHI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("HeikinAshi") // From catalog.bak.json
            .WithId("HEIKINASHI") // From catalog.bak.json
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform) // From catalog.bak.json Category: "PriceTransform"
            .WithMethodName("ToHeikinAshi")
                                                   // No parameters for HEIKINASHI in catalog.bak.json
            .AddResult("Open", "Open", ResultType.Default, isDefault: false) // From HeikinAshiResult model
            .AddResult("High", "High", ResultType.Default, isDefault: false) // From HeikinAshiResult model
            .AddResult("Low", "Low", ResultType.Default, isDefault: false) // From HeikinAshiResult model
            .AddResult("Close", "Close", ResultType.Default, isDefault: false) // From HeikinAshiResult model
            .AddResult("Volume", "Volume", ResultType.Default, isDefault: false) // From HeikinAshiResult model
            .Build();

    // No StreamListing for HEIKINASHI.
    // No BufferListing for HEIKINASHI.
}
