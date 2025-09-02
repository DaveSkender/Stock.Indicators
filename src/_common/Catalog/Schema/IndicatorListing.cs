using System.ComponentModel.DataAnnotations;

namespace Skender.Stock.Indicators;
#pragma warning disable CA1308 // Normalize strings to uppercase

/// <summary>
/// Represents an indicator listing with its configuration and parameters.
/// </summary>
[Serializable]
public record IndicatorListing
{
    private readonly string _baseUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorListing"/> class.
    /// </summary>
    public IndicatorListing() : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorListing"/> class with the specified base URL.
    /// </summary>
    /// <param name="baseUrl">Optional base for <see cref="UiidEndpoint"/></param>
    internal IndicatorListing(
        string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    /// <summary>
    /// Gets or sets the name of the indicator.
    /// </summary>
    [MinLength(5)]
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the indicator.
    /// </summary>
    [MinLength(2), UrlSafe]
    public required string Uiid { get; init; }

    /// <summary>
    /// Gets or sets the style of the indicator (Series, Buffer, Stream).
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Style Style { get; init; } = Style.Series;

    /// <summary>
    /// Gets or sets the category of the indicator.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Category Category { get; init; }

    /// <summary>
    /// Gets or sets the collection of parameters for the indicator.
    /// </summary>
    public IReadOnlyList<IndicatorParam>? Parameters { get; init; }

    /// <summary>
    /// Gets or sets the return type name for the indicator method.
    /// </summary>
    public string? ReturnType { get; init; }

    /// <summary>
    /// Gets or sets the method name for automation use cases.
    /// </summary>
    public string? MethodName { get; init; }

    /// <summary>
    /// Gets or sets the collection of result configurations for the indicator.
    /// </summary>
    public required IReadOnlyList<IndicatorResult> Results { get; init; }

    /// <summary>
    /// Gets or sets a hypothetical endpoint URL for the indicator.
    /// </summary>
    [MinLength(2), UrlSafe]
    public string UiidEndpoint => $"{_baseUrl}/{Uiid?.ToLowerInvariant()}";

    /// <summary>
    /// Gets or sets the legend template for the indicator.
    /// </summary>
    public required string LegendTemplate { get; init; }
}
