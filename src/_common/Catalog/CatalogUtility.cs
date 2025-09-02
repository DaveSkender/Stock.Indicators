using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Skender.Stock.Indicators;

/// <summary>
/// Utility class for executing indicators with minimal configuration requirements.
/// Provides simple methods to execute indicators using only ID and Style without requiring
/// prior knowledge of result types or implementation syntax.
/// </summary>
public static class CatalogUtility
{
    private static readonly JsonSerializerOptions JsonOptions = new() {
        PropertyNameCaseInsensitive = true,
        Converters = { new StyleJsonConverter() }
    };

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
            config = JsonSerializer.Deserialize<IndicatorConfig>(json, JsonOptions)
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

    /// <summary>
    /// Custom JSON converter for Style enum that accepts either integer values or string names (case-insensitive).
    /// </summary>
    private sealed class StyleJsonConverter : JsonConverter<Style>
    {
        public override Style Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                int numeric = reader.GetInt32();
                return Enum.IsDefined(typeof(Style), numeric)
                    ? (Style)numeric
                    : throw new JsonException($"Invalid Style value: {numeric}");
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? str = reader.GetString();
                return !string.IsNullOrWhiteSpace(str) && Enum.TryParse(str, ignoreCase: true, out Style parsed)
                    ? parsed
                    : throw new JsonException($"Invalid Style value: '{str}'");
            }

            throw new JsonException($"Unexpected token parsing Style: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, Style value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Extension methods for catalog export functionality.
/// </summary>
public static class CatalogExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Converts the catalog to a JSON string representation.
    /// </summary>
    /// <param name="catalog">The catalog of indicator listings.</param>
    /// <param name="filePath">Optional file path to save the JSON output. If provided, the content will be written to this file.</param>
    /// <returns>A JSON string containing all catalog properties according to the defined schema.</returns>
    public static string ToJson(
        this IReadOnlyCollection<IndicatorListing> catalog,
        string? filePath = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        string json = JsonSerializer.Serialize(catalog, JsonOptions);

        // Save to file if path is provided
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            File.WriteAllText(filePath, json);
        }

        return json;
    }

    /// <summary>
    /// Converts the catalog to a Markdown string representation.
    /// </summary>
    /// <param name="catalog">The catalog of indicator listings.</param>
    /// <param name="asChecklist">The markdown format type - checklist (true, default) or table (false).</param>
    /// <param name="filePath">
    /// Optional file path to save the Markdown output.
    /// If provided, the content will be written to this file.
    /// </param>
    /// <returns>A Markdown string representation of the catalog.</returns>
    public static string ToMarkdown(
        this IReadOnlyCollection<IndicatorListing> catalog,
        bool asChecklist = true,
        string? filePath = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        string markdown = asChecklist
            ? ToMarkdownChecklist(catalog)
            : ToMarkdownTable(catalog);

        // Save to file if path is provided
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            File.WriteAllText(filePath, markdown);
        }

        return markdown;
    }

    /// <summary>
    /// Converts the catalog to a Markdown checklist format.
    /// </summary>
    /// <param name="catalog">The catalog of indicator listings.</param>
    /// <returns>A Markdown checklist string with format: - [ ] {id}: {name} ({available types})</returns>
    private static string ToMarkdownChecklist(IReadOnlyCollection<IndicatorListing> catalog)
    {
        StringBuilder sb = new();

        // Group by ID to show all styles for each indicator
        IOrderedEnumerable<IGrouping<string, IndicatorListing>> groupedIndicators = catalog
            .GroupBy(listing => listing.Uiid)
            .OrderBy(group => group.Key);

        foreach (IGrouping<string, IndicatorListing>? group in groupedIndicators)
        {
            IndicatorListing firstListing = group.First();
            string availableTypes = string.Join(", ", group.Select(l => l.Style.ToString()).OrderBy(s => s));

            sb.AppendLine(CultureInfo.InvariantCulture, $"- [ ] {group.Key}: {firstListing.Name} ({availableTypes})");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Converts the catalog to a Markdown table format.
    /// </summary>
    /// <param name="catalog">The catalog of indicator listings.</param>
    /// <returns>A Markdown table with columns for ID, Name, Series, Buffer, Stream.</returns>
    private static string ToMarkdownTable(IReadOnlyCollection<IndicatorListing> catalog)
    {
        StringBuilder sb = new();

        // Group by ID to consolidate multiple styles into single rows
        IOrderedEnumerable<IGrouping<string, IndicatorListing>> groupedIndicators = catalog
            .GroupBy(listing => listing.Uiid)
            .OrderBy(group => group.Key);

        // Table header
        sb.AppendLine("| ID | Name | Series | Buffer | Stream |");
        sb.AppendLine("|---|---|:---:|:---:|:---:|");

        // Table rows
        foreach (IGrouping<string, IndicatorListing>? group in groupedIndicators)
        {
            IndicatorListing firstListing = group.First();
            List<IndicatorListing> styles = group.ToList();

            // Check which styles are available
            string hasSeries = styles.Any(s => s.Style == Style.Series) ? "✓" : "";
            string hasBuffer = styles.Any(s => s.Style == Style.Buffer) ? "✓" : "";
            string hasStream = styles.Any(s => s.Style == Style.Stream) ? "✓" : "";

            sb.AppendLine(CultureInfo.InvariantCulture, $"| {group.Key} | {firstListing.Name} | {hasSeries} | {hasBuffer} | {hasStream} |");
        }

        return sb.ToString().TrimEnd();
    }
}
