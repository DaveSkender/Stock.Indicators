using System.Text.Json;
using System.Text.Json.Serialization;
using Tests.Indicators.Baselines.Models;

namespace Tests.Indicators.Baselines;

/// <summary>
/// Provides JSON serialization utilities for baseline files.
/// </summary>
public static class BaselineWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Converters = { new DateOnlyJsonConverter() }
    };

    /// <summary>
    /// Serializes a baseline file to JSON string.
    /// </summary>
    /// <param name="baselineFile">The baseline file to serialize.</param>
    /// <returns>JSON string representation.</returns>
    public static string ToJson(BaselineFile baselineFile)
    {
        ArgumentNullException.ThrowIfNull(baselineFile);
        return JsonSerializer.Serialize(baselineFile, SerializerOptions);
    }

    /// <summary>
    /// Writes a baseline file to disk as JSON.
    /// </summary>
    /// <param name="baselineFile">The baseline file to write.</param>
    /// <param name="filePath">The destination file path.</param>
    public static void WriteToFile(BaselineFile baselineFile, string filePath)
    {
        ArgumentNullException.ThrowIfNull(baselineFile);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string json = ToJson(baselineFile);
        string directory = Path.GetDirectoryName(filePath)
            ?? throw new ArgumentException("Invalid file path.", nameof(filePath));

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
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
