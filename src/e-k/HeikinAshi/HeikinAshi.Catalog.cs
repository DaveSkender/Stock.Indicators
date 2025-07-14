namespace Skender.Stock.Indicators;

public static partial class HeikinAshi
{
    // HEIKINASHI Series Listing
    internal static readonly IndicatorListing SeriesListing =
        new IndicatorListingBuilder()
            .WithName("HeikinAshi")
            .WithId("HEIKINASHI")
            .WithStyle(Style.Series)
            .WithCategory(Category.PriceTransform)
            .WithMethodName("ToHeikinAshi")
            .AddResult("Open", "Open", ResultType.Default, isDefault: false)
            .AddResult("High", "High", ResultType.Default, isDefault: false)
            .AddResult("Low", "Low", ResultType.Default, isDefault: false)
            .AddResult("Close", "Close", ResultType.Default, isDefault: true)
            .AddResult("Volume", "Volume", ResultType.Default, isDefault: false)
            .Build();

    // No StreamListing for HEIKINASHI.
    // No BufferListing for HEIKINASHI.
}
