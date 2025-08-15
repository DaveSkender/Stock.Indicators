using System.Globalization;
using System.Text.Json;

namespace Skender.Stock.Indicators;

/// <summary>
/// Utility class for executing indicators with minimal configuration requirements.
/// Provides simple methods to execute indicators using only ID and Style without requiring
/// prior knowledge of result types or implementation syntax.
/// </summary>
public static class CatalogUtility
{
    /// <summary>
    /// Executes an indicator using only its ID and style.
    /// </summary>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="id">The indicator ID (e.g., "EMA", "RSI", "MACD").</param>
    /// <param name="style">The indicator style (Series, Stream, or Buffer).</param>
    /// <param name="parameters">Optional parameter overrides.</param>
    /// <returns>The indicator results as a list of objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null.</exception>
    /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found or executed.</exception>
    public static IReadOnlyList<object> ExecuteById(
        this IEnumerable<IQuote> quotes,
        string id,
        Style style,
        Dictionary<string, object>? parameters = null)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(quotes);
        
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Indicator ID cannot be null or empty.", nameof(id));
        }

        // Find the indicator listing
        IndicatorListing? listing = IndicatorRegistry.GetByIdAndStyle(id, style);
        if (listing == null)
        {
            throw new InvalidOperationException($"Indicator '{id}' with style '{style}' not found in registry.");
        }

        // Execute using the existing IndicatorExecutor with dynamic result type determination
        return ExecuteWithDynamicType(quotes, listing, parameters);
    }

    /// <summary>
    /// Executes an indicator from a JSON configuration string.
    /// </summary>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="json">The JSON configuration string containing indicator settings.</param>
    /// <returns>The indicator results as a list of objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when json or quotes is null.</exception>
    /// <exception cref="ArgumentException">Thrown when json is empty or invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found or executed.</exception>
    public static IReadOnlyList<object> ExecuteFromJson(
        this IEnumerable<IQuote> quotes,
        string json)
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(quotes);
        ArgumentNullException.ThrowIfNull(json);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON configuration cannot be null or empty.", nameof(json));
        }

        // Parse the JSON configuration
        IndicatorConfig config;
        try
        {
            config = JsonSerializer.Deserialize<IndicatorConfig>(json) 
                ?? throw new ArgumentException("Failed to parse JSON configuration - result was null.", nameof(json));
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON configuration: {ex.Message}", nameof(json), ex);
        }

        // Convert JsonElement values to proper types in Parameters
        var convertedParameters = ConvertJsonElementsInParameters(config.Parameters);

        // Execute using the parsed configuration
        return quotes.ExecuteById(config.Id, config.Style, convertedParameters);
    }

    /// <summary>
    /// Executes an indicator with dynamic result type determination.
    /// </summary>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="listing">The indicator listing.</param>
    /// <param name="parameters">Optional parameter overrides.</param>
    /// <returns>The indicator results as a list of objects.</returns>
    private static List<object> ExecuteWithDynamicType(
        IEnumerable<IQuote> quotes,
        IndicatorListing listing,
        Dictionary<string, object>? parameters)
    {
        // Get the result type from the listing's method signature
        string? resultTypeName = GetResultTypeFromListing(listing);
        if (resultTypeName == null)
        {
            throw new InvalidOperationException($"Cannot determine result type for indicator '{listing.Uiid}'.");
        }

        // Get the result type from the assembly
        Type? resultType = GetResultType(resultTypeName);
        if (resultType == null)
        {
            throw new InvalidOperationException($"Result type '{resultTypeName}' not found for indicator '{listing.Uiid}'.");
        }

        // Use reflection to call the generic Execute method
        System.Reflection.MethodInfo? executeMethod = typeof(IndicatorExecutor)
            .GetMethods()
            .FirstOrDefault(m => m.Name == "Execute" && 
                           m.IsGenericMethodDefinition && 
                           m.GetParameters().Length == 3 &&
                           m.GetParameters()[2].ParameterType == typeof(Dictionary<string, object>));

        if (executeMethod == null)
        {
            throw new InvalidOperationException("Could not find appropriate Execute method in IndicatorExecutor.");
        }

        // Make the method generic with the quote type and result type
        System.Reflection.MethodInfo genericMethod = executeMethod.MakeGenericMethod(typeof(Quote), resultType);

        // Execute the method
        object? result = genericMethod.Invoke(null, [quotes, listing, parameters]);
        
        if (result is not IEnumerable<object> objectList)
        {
            // Convert the result to a list of objects
            if (result is System.Collections.IEnumerable enumerable)
            {
                List<object> convertedList = [];
                foreach (object item in enumerable)
                {
                    convertedList.Add(item);
                }
                return convertedList;
            }
            throw new InvalidOperationException("Method execution did not return a valid result collection.");
        }

        return objectList.ToList();
    }

    /// <summary>
    /// Extracts the result type name from an indicator listing.
    /// </summary>
    /// <param name="listing">The indicator listing.</param>
    /// <returns>The result type name, or null if it cannot be determined.</returns>
    private static string? GetResultTypeFromListing(IndicatorListing listing)
    {
        // For most indicators, the result type follows the pattern: {IndicatorName}Result
        // For example: EMA -> EmaResult, RSI -> RsiResult, MACD -> MacdResult
        string? indicatorName = listing.Uiid;
        if (string.IsNullOrEmpty(indicatorName))
        {
            return null;
        }

        // Convert to proper casing for type name (first letter uppercase, rest lowercase)
        // CA1308: This is intentional for class name formatting, not string comparison
#pragma warning disable CA1308
        string typeName = char.ToUpper(indicatorName[0], CultureInfo.InvariantCulture) + 
                         indicatorName[1..].ToLowerInvariant() + "Result";
#pragma warning restore CA1308
        
        // Handle special cases where the pattern doesn't match
        return typeName switch
        {
            "BollingerResult" => "BollingerBandsResult",
            _ => typeName
        };
    }

    /// <summary>
    /// Gets a result type by name from the indicators assembly.
    /// </summary>
    /// <param name="typeName">The name of the type to find.</param>
    /// <returns>The Type if found, otherwise null.</returns>
    private static Type? GetResultType(string typeName)
    {
        // Get the assembly containing the indicators
        System.Reflection.Assembly indicatorsAssembly = typeof(Ema).Assembly;
        
        // Search for the type in the assembly
        Type? resultType = indicatorsAssembly.GetTypes()
            .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        return resultType;
    }

    /// <summary>
    /// Converts JsonElement values in parameters dictionary to their appropriate types.
    /// </summary>
    /// <param name="parameters">The parameters dictionary that may contain JsonElement values.</param>
    /// <returns>A new dictionary with converted values.</returns>
    private static Dictionary<string, object>? ConvertJsonElementsInParameters(Dictionary<string, object>? parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return parameters;
        }

        var converted = new Dictionary<string, object>();
        foreach (var kvp in parameters)
        {
            if (kvp.Value is JsonElement jsonElement)
            {
                object? convertedValue = ConvertJsonElement(jsonElement);
                // Skip null values to maintain Dictionary<string, object> compatibility
                if (convertedValue != null)
                {
                    converted[kvp.Key] = convertedValue;
                }
            }
            else
            {
                converted[kvp.Key] = kvp.Value;
            }
        }

        return converted;
    }

    /// <summary>
    /// Converts a JsonElement to its appropriate .NET type.
    /// </summary>
    /// <param name="element">The JsonElement to convert.</param>
    /// <returns>The converted value, or null for null JsonElements.</returns>
    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number when element.TryGetInt32(out int intValue) => intValue,
            JsonValueKind.Number when element.TryGetDouble(out double doubleValue) => doubleValue,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}