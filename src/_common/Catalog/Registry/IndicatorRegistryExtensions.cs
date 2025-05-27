namespace Skender.Stock.Indicators;

/// <summary>
/// Extension methods for common indicator registry queries.
/// </summary>
public static class IndicatorRegistryExtensions
{
    /// <summary>
    /// Searches for indicators by partial name match.
    /// </summary>
    /// <param name="partialName">Partial name to search for.</param>
    /// <returns>A read-only collection of indicators matching the partial name.</returns>
    public static IReadOnlyCollection<IndicatorListing> SearchByName(string partialName)
        => IndicatorRegistry.Search(partialName);

    /// <summary>
    /// Searches for indicators by partial name match with style filtering.
    /// </summary>
    /// <param name="partialName">Partial name to search for.</param>
    /// <param name="style">Style to filter by.</param>
    /// <returns>A read-only collection of indicators matching the partial name and style.</returns>
    public static IReadOnlyCollection<IndicatorListing> SearchByName(string partialName, Style style)
        => IndicatorRegistry.Search(partialName, style);

    /// <summary>
    /// Searches for indicators by partial name match with category filtering.
    /// </summary>
    /// <param name="partialName">Partial name to search for.</param>
    /// <param name="category">Category to filter by.</param>
    /// <returns>A read-only collection of indicators matching the partial name and category.</returns>
    public static IReadOnlyCollection<IndicatorListing> SearchByName(string partialName, Category category)
        => IndicatorRegistry.Search(partialName, null, category);

    /// <summary>
    /// Gets indicators that have a specific parameter.
    /// </summary>
    /// <param name="parameterName">The parameter name to search for.</param>
    /// <returns>A read-only collection of indicators that have the specified parameter.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithParameter(string parameterName)
        => string.IsNullOrWhiteSpace(parameterName)
            ? new List<IndicatorListing>().AsReadOnly()
            : IndicatorRegistry.GetAllIndicators()
                .Where(indicator => indicator.Parameters != null
                    && indicator.Parameters.Any(param
                        => param.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase)))
                .ToList()
                .AsReadOnly();

    /// <summary>
    /// Gets indicators that require a specific parameter type.
    /// </summary>
    /// <typeparam name="T">The parameter type to search for.</typeparam>
    /// <returns>A read-only collection of indicators that have parameters of the specified type.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithParameterType<T>()
    {
        Type targetType = typeof(T);

        return IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Parameters != null
                && indicator.Parameters.Any(param
                    => param.DataType == targetType.ToString()))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets indicators by their result count.
    /// </summary>
    /// <param name="resultCount">The number of results to filter by.</param>
    /// <returns>A read-only collection of indicators that produce the specified number of results.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsByResultCount(int resultCount)
        => IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Results.Count == resultCount)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets indicators that have a default result.
    /// </summary>
    /// <returns>A read-only collection of indicators that have a default result.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithDefaultResult()
        => IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Results.Any(result => result.IsDefault))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets indicators that have a specific result type.
    /// </summary>
    /// <param name="resultType">The result type to filter by.</param>
    /// <returns>A read-only collection of indicators that produce the specified result type.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithResultType(ResultType resultType)
        => IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Results.Any(result => result.DataType == resultType))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets indicators that have a result with a specific name.
    /// </summary>
    /// <param name="resultName">The result name to filter by.</param>
    /// <returns>A read-only collection of indicators that have a result with the specified name.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithResultName(string resultName)
        => string.IsNullOrWhiteSpace(resultName)
            ? new List<IndicatorListing>().AsReadOnly()
            : IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Results.Any(result =>
                (result.DataName != null && result.DataName.Contains(resultName, StringComparison.OrdinalIgnoreCase)) ||
                (result.DisplayName != null && result.DisplayName.Contains(resultName, StringComparison.OrdinalIgnoreCase))))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets indicators sorted by name.
    /// </summary>
    /// <param name="ascending">True for ascending sort; false for descending.</param>
    /// <returns>A read-only collection of indicators sorted by name.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsSortedByName(bool ascending = true)
        => ascending
            ? IndicatorRegistry.GetAllIndicators()
                .OrderBy(indicator => indicator.Name)
                .ToList()
                .AsReadOnly()
            : IndicatorRegistry.GetAllIndicators()
                .OrderByDescending(indicator => indicator.Name)
                .ToList()
                .AsReadOnly();

    /// <summary>
    /// Gets indicators with required parameters.
    /// </summary>
    /// <returns>A read-only collection of indicators that have at least one required parameter.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithRequiredParameters()
        => IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Parameters != null &&
                   indicator.Parameters.Any(param => param.IsRequired))
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets indicators with optional parameters (parameters with default values).
    /// </summary>
    /// <returns>A read-only collection of indicators that have at least one optional parameter.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicatorsWithOptionalParameters()
        => IndicatorRegistry.GetAllIndicators()
            .Where(indicator => indicator.Parameters != null &&
                   indicator.Parameters.Any(param => !param.IsRequired))
            .ToList()
            .AsReadOnly();
}
