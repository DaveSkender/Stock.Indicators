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
}
