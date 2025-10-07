using System.Text.Json;
using System.Text.Json.Serialization;
using Tests.Indicators.Baselines.Models;

namespace Tests.Indicators.Baselines;

/// <summary>
/// Provides JSON deserialization utilities for baseline files.
/// </summary>
public static class BaselineReader
{
    private static readonly JsonSerializerOptions DeserializerOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        Converters = { new DateOnlyJsonConverter() }
    };

    /// <summary>
    /// Deserializes a JSON string to a baseline file.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized baseline file.</returns>
    /// <exception cref="JsonException">Thrown when JSON is malformed or missing required fields.</exception>
    public static BaselineFile FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        try
        {
#nullable enable
            BaselineFile? baselineFile = JsonSerializer.Deserialize<BaselineFile>(json, DeserializerOptions);
#nullable disable

            if (baselineFile is null)
            {
                throw new JsonException("Failed to deserialize baseline file: result is null.");
            }

            ValidateBaselineFile(baselineFile);
            return baselineFile;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Malformed baseline JSON: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Reads and deserializes a baseline file from disk.
    /// </summary>
    /// <param name="filePath">The file path to read from.</param>
    /// <returns>The deserialized baseline file, or null if file does not exist.</returns>
    /// <exception cref="JsonException">Thrown when JSON is malformed or missing required fields.</exception>
#nullable enable
    public static BaselineFile? ReadFromFile(string filePath)
#nullable disable
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            return FromJson(json);
        }
        catch (JsonException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new JsonException($"Failed to read baseline file '{filePath}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validates that a baseline file has all required fields.
    /// </summary>
    private static void ValidateBaselineFile(BaselineFile baselineFile)
    {
        if (string.IsNullOrWhiteSpace(baselineFile.Metadata.IndicatorName))
        {
            throw new JsonException("Baseline metadata missing required field: IndicatorName");
        }

        if (string.IsNullOrWhiteSpace(baselineFile.Metadata.ScenarioName))
        {
            throw new JsonException("Baseline metadata missing required field: ScenarioName");
        }

        if (string.IsNullOrWhiteSpace(baselineFile.Metadata.LibraryVersion))
        {
            throw new JsonException("Baseline metadata missing required field: LibraryVersion");
        }

        if (baselineFile.Results.Count == 0)
        {
            throw new JsonException("Baseline file must contain at least one result.");
        }
    }

    /// <summary>
    /// Custom JSON converter for DateTime to ensure ISO 8601 date-only format.
    /// </summary>
    private sealed class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
#nullable enable
            string? dateString = reader.GetString();
#nullable disable
            return dateString is not null
                ? DateTime.Parse(dateString, System.Globalization.CultureInfo.InvariantCulture)
                : throw new JsonException("Date value is null or invalid.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
