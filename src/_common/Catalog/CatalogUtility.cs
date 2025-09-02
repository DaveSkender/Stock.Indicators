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
    /// Executes an indicator using only its ID and style with a typed result.
    /// </summary>
    /// <typeparam name="TResult">The expected indicator result type.</typeparam>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="id">The indicator ID (e.g., "EMA", "RSI", "MACD").</param>
    /// <param name="style">The indicator style (Series, Stream, or Buffer).</param>
    /// <param name="parameters">Optional parameter overrides.</param>
    /// <returns>The indicator results as a typed list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null.</exception>
    /// <exception cref="ArgumentException">Thrown when id is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found or executed.</exception>
    public static IReadOnlyList<TResult> ExecuteById<TResult>(
        this IEnumerable<IQuote> quotes,
        string id,
        Style style,
        Dictionary<string, object>? parameters = null)
        where TResult : class
    {
        // Validate inputs
        ArgumentNullException.ThrowIfNull(quotes);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Indicator ID cannot be null or empty.", nameof(id));
        }

        // Find the indicator listing
        IndicatorListing? listing = IndicatorRegistry.GetByIdAndStyle(id, style)
            ?? throw new InvalidOperationException($"Indicator '{id}' with style '{style}' not found in registry.");

        // Execute using typed executor
        return IndicatorExecutor.Execute<IQuote, TResult>(quotes, listing, parameters);
    }



    /// <summary>
    /// Executes an indicator from a JSON configuration string with a typed result.
    /// </summary>
    /// <typeparam name="TResult">The expected indicator result type.</typeparam>
    /// <param name="quotes">The quotes to process.</param>
    /// <param name="json">The JSON configuration string containing indicator settings.</param>
    /// <returns>The indicator results as a typed list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when json or quotes is null.</exception>
    /// <exception cref="ArgumentException">Thrown when json is empty or invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the indicator cannot be found or executed.</exception>
    public static IReadOnlyList<TResult> ExecuteFromJson<TResult>(
        this IEnumerable<IQuote> quotes,
        string json)
        where TResult : class
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
        Dictionary<string, object>? convertedParameters = ConvertJsonElementsInParameters(config.Parameters);

        // Execute using the parsed configuration
        return quotes.ExecuteById<TResult>(config.Id, config.Style, convertedParameters);
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

        Dictionary<string, object> converted = [];
        foreach (KeyValuePair<string, object> kvp in parameters)
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
        => element.ValueKind switch {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number when element.TryGetInt32(out int intValue) => intValue,
            JsonValueKind.Number when element.TryGetDecimal(out decimal decimalValue) => decimalValue,
            JsonValueKind.Number when element.TryGetDouble(out double doubleValue) => doubleValue,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
}
