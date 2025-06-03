namespace Skender.Stock.Indicators;

public static partial class IndicatorCatalog
{
    // No pre-populated catalog - tests will register specific indicators as needed
    private static readonly List<IndicatorListing> _catalog = [];

    public static IReadOnlyList<IndicatorListing> GetCatalog()
        => _catalog;
}
