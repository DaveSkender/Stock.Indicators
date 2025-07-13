namespace Skender.Stock.Indicators;

/// <summary>
/// Extension methods for IndicatorRegistry.
/// </summary>
public static class IndicatorRegistryExtensions
{
    /// <summary>
    /// Searches for indicators by name, with an optional style filter.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="style">Optional style filter.</param>
    /// <returns>All indicator listings that match the search query and style filter.</returns>
    public static IReadOnlyCollection<IndicatorListing> SearchByName(string query, Style style)
    {
        IReadOnlyCollection<IndicatorListing> allResults = IndicatorRegistry.Search(query);

        return allResults
            .Where(x => x.Style == style)
            .ToList();
    }

    /// <summary>
    /// Searches for indicators by name, with an optional category filter.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="category">Optional category filter.</param>
    /// <returns>All indicator listings that match the search query and category filter.</returns>
    public static IReadOnlyCollection<IndicatorListing> SearchByName(string query, Category category)
    {
        IReadOnlyCollection<IndicatorListing> allResults = IndicatorRegistry.Search(query);

        return allResults
            .Where(x => x.Category == category)
            .ToList();
    }

    /// <summary>
    /// Gets all indicators with a specific result type.
    /// </summary>
    /// <param name="resultType">The result type to filter by.</param>
    /// <returns>All indicator listings with the specified result type.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithResultType(ResultType resultType)
    {
        IReadOnlyCollection<IndicatorListing> allIndicators = IndicatorRegistry.GetIndicators();

        return allIndicators
            .Where(i => i.Results != null && i.Results.Any(r => r.DataType == resultType))
            .ToList();
    }

    /// <summary>
    /// Gets all indicators with result names containing a specific string.
    /// </summary>
    /// <param name="resultNamePart">The string to search for in result names.</param>
    /// <returns>All indicator listings with matching result names.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithResultName(string resultNamePart)
    {
        IReadOnlyCollection<IndicatorListing> allIndicators = IndicatorRegistry.GetIndicators();

        return allIndicators
            .Where(i => i.Results != null && i.Results.Any(r =>
                r.DataName.Contains(resultNamePart, StringComparison.OrdinalIgnoreCase) ||
                r.DisplayName.Contains(resultNamePart, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }

    /// <summary>
    /// Gets all indicators sorted by name.
    /// </summary>
    /// <param name="ascending">Sort direction, true for ascending, false for descending.</param>
    /// <returns>All indicator listings sorted by name.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsSortedByName(bool ascending = true)
    {
        IReadOnlyCollection<IndicatorListing> allIndicators = IndicatorRegistry.GetIndicators();

        return ascending
            ? allIndicators.OrderBy(i => i.Name).ToList()
            : allIndicators.OrderByDescending(i => i.Name).ToList();
    }

    /// <summary>
    /// Gets all indicators that have required parameters.
    /// </summary>
    /// <returns>All indicator listings with required parameters.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithRequiredParameters()
    {
        IReadOnlyCollection<IndicatorListing> allIndicators = IndicatorRegistry.GetIndicators();

        return allIndicators
            .Where(i => i.Parameters != null && i.Parameters.Any(p => p.IsRequired))
            .ToList();
    }

    /// <summary>
    /// Gets all indicators that have only optional parameters.
    /// </summary>
    /// <returns>All indicator listings with only optional parameters.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithOptionalParameters()
    {
        IReadOnlyCollection<IndicatorListing> allIndicators = IndicatorRegistry.GetIndicators();

        return allIndicators
            .Where(i => i.Parameters != null && !i.Parameters.Any(p => p.IsRequired))
            .ToList();
    }
}
