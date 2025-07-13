namespace Skender.Stock.Indicators;

/// <summary>
/// Provides a catalog of all registered indicators.
/// </summary>
/// <remarks>The <see cref="IndicatorCatalog"/> class maintains a collection of indicators that can be accessed
/// through the <see cref="Catalog"/> property. This class is static and cannot be instantiated.</remarks>
public static partial class IndicatorCatalog
{
    // No pre-populated catalog - tests will register specific indicators as needed
    private static readonly List<IndicatorListing> _catalog = [];

    /// <summary>
    /// Catalog of all indicators
    /// </summary>
    public static IReadOnlyList<IndicatorListing> Catalog => _catalog;
}
