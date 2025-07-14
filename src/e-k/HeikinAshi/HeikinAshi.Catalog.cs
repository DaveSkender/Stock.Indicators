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
            .AddResult("Open", "Open", ResultType.Default)
            .AddResult("High", "High", ResultType.Default)
            .AddResult("Low", "Low", ResultType.Default)
            .AddResult("Close", "Close", ResultType.Default, isReusable: true)
            .AddResult("Volume", "Volume", ResultType.Default)
            .Build();

    // No StreamListing for HEIKINASHI.
    // No BufferListing for HEIKINASHI.
}
