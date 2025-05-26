using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for working with indicator metadata and exporting catalog information.
/// </summary>
public static class Metadata
{
    private static readonly JsonSerializerOptions _jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Exports the complete indicator catalog to a JSON string.
    /// </summary>
    /// <param name="includeChartConfig">Whether to include chart configuration data in the output. Default is true.</param>
    /// <returns>A JSON string representation of the indicator catalog.</returns>
    public static string ToJson(bool includeChartConfig = true)
    {
        IReadOnlyList<IndicatorListing> catalog = includeChartConfig
            ? Catalog.Get()
            : FilterChartConfig(Catalog.Get());

        return JsonSerializer.Serialize(catalog, _jsonOptions);
    }

    /// <summary>
    /// Exports the indicator catalog to a JSON string, with URLs for API endpoints.
    /// </summary>
    /// <param name="baseUrl">The base URL for the API endpoints.</param>
    /// <param name="includeChartConfig">Whether to include chart configuration data in the output. Default is true.</param>
    /// <returns>A JSON string representation of the indicator catalog with API endpoints.</returns>
    public static string ToJson(Uri baseUrl, bool includeChartConfig = true)
    {
        IReadOnlyList<IndicatorListing> catalog = includeChartConfig
            ? Catalog.Get(baseUrl)
            : FilterChartConfig(Catalog.Get(baseUrl));

        return JsonSerializer.Serialize(catalog, _jsonOptions);
    }

    /// <summary>
    /// Gets indicator metadata by indicator ID.
    /// </summary>
    /// <param name="indicatorId">The unique identifier of the indicator (e.g., "SMA").</param>
    /// <param name="includeChartConfig">Whether to include chart configuration data in the output. Default is true.</param>
    /// <returns>The indicator listing for the specified ID, or null if not found.</returns>
    public static IndicatorListing? GetById(string indicatorId, bool includeChartConfig = true)
    {
        if (string.IsNullOrEmpty(indicatorId))
        {
            return null;
        }

        IndicatorListing? listing = Catalog.Get()
            .FirstOrDefault(x => x.Uiid.Equals(indicatorId, StringComparison.OrdinalIgnoreCase));

        if (listing != null && !includeChartConfig)
        {
            return FilterChartConfig(listing);
        }

        return listing;
    }

    /// <summary>
    /// Gets metadata for the indicator by method name.
    /// </summary>
    /// <param name="methodName">The method name for which to retrieve metadata.</param>
    /// <param name="includeChartConfig">Whether to include chart configuration data in the output. Default is true.</param>
    /// <returns>The indicator listing for the specified method, or null if not found.</returns>
    public static IndicatorListing? GetByMethod(string methodName, bool includeChartConfig = true)
    {
        if (string.IsNullOrEmpty(methodName))
        {
            return null;
        }

        // Extract method name from the qualified name if it includes the class
        string method = methodName.Contains('.')
            ? methodName.Split('.').Last()
            : methodName;

        // Find by exact match
        IndicatorListing? listing = Catalog.Get()
            .FirstOrDefault(x => ContainsMethod(x, method));

        if (listing != null && !includeChartConfig)
        {
            return FilterChartConfig(listing);
        }

        return listing;
    }

    /// <summary>
    /// Filters chart configuration data from the indicator listings.
    /// </summary>
    /// <param name="listings">The indicator listings to filter.</param>
    /// <returns>Filtered indicator listings without chart configuration data.</returns>
    private static IReadOnlyList<IndicatorListing> FilterChartConfig(IReadOnlyList<IndicatorListing> listings)
    {
        List<IndicatorListing> filtered = [];

        foreach (IndicatorListing listing in listings)
        {
            filtered.Add(FilterChartConfig(listing));
        }

        return filtered;
    }

    /// <summary>
    /// Filters chart configuration data from a single indicator listing.
    /// </summary>
    /// <param name="listing">The indicator listing to filter.</param>
    /// <returns>Filtered indicator listing without chart configuration data.</returns>
    private static IndicatorListing FilterChartConfig(IndicatorListing listing)
    {
        // Create a new instance with no baseUrl
        // We need to ensure all required properties are initialized
        return new IndicatorListing() {
            Name = listing.Name,
            Uiid = listing.Uiid,
            Category = listing.Category,
            ChartType = listing.ChartType,
            Style = listing.Style,
            ReturnType = listing.ReturnType,
            Order = listing.Order,
            Parameters = listing.Parameters,
            Results = RemoveChartStyles(listing.Results),
            LegendTemplate = listing.LegendTemplate
        };
    }

    /// <summary>
    /// Removes chart styles from indicator result configurations.
    /// </summary>
    /// <param name="results">The result configurations to process.</param>
    /// <returns>Result configurations without chart styles.</returns>
    private static List<IndicatorResultConfig> RemoveChartStyles(IReadOnlyList<IndicatorResultConfig> results)
    {
        List<IndicatorResultConfig> filtered = [];

        foreach (var result in results)
        {
            filtered.Add(new IndicatorResultConfig {
                DisplayName = result.DisplayName,
                DataName = result.DataName,
                DataType = result.DataType,
                TooltipTemplate = result.TooltipTemplate,
                IsDataPoint = result.IsDataPoint,
                IsDefaultOutput = result.IsDefaultOutput
            });
        }

        return filtered;
    }

    /// <summary>
    /// Checks if the indicator listing contains the specified method.
    /// </summary>
    /// <param name="listing">The indicator listing to check.</param>
    /// <param name="methodName">The method name to search for.</param>
    /// <returns>True if the indicator listing contains the method, otherwise false.</returns>
    private static bool ContainsMethod(IndicatorListing listing, string methodName)
    {
        // The method name information should be added to the catalog generator
        // For now, we'll use a simple heuristic based on the indicator ID
        string id = listing.Uiid.ToUpperInvariant();
        string method = methodName.ToUpperInvariant();

        return string.Equals(id, method, StringComparison.OrdinalIgnoreCase) || // Direct match
               method.StartsWith(id, StringComparison.OrdinalIgnoreCase) || // Method starts with indicator id
               id.EndsWith(method, StringComparison.OrdinalIgnoreCase); // Indicator id ends with method
    }
}
