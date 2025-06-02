namespace Skender.Stock.Indicators;

public static partial class IndicatorCatalog
{
    private static readonly List<IndicatorListing> _catalog = [
        Ema.SeriesListing,
        Ema.StreamListing,
        Ema.BufferListing
        // Sma.SeriesListing,
        // Sma.StreamListing,
        // Sma.BufferListing,
    ];

    public static IReadOnlyList<IndicatorListing> GetCatalog()
        => _catalog;
}
