using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a composite indicator listing that aggregates multiple style variants into a single listing.
/// This enables indicators that support multiple styles (Series, Stream, and Buffer) to have a unified catalog entry.
/// </summary>
[Serializable]
public record CompositeIndicatorListing : IndicatorListing
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIndicatorListing"/> class.
    /// </summary>
    public CompositeIndicatorListing() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeIndicatorListing"/> class with the specified base URL.
    /// </summary>
    /// <param name="baseUrl">Optional base for endpoint URL</param>
    internal CompositeIndicatorListing(string baseUrl) : base(baseUrl) { }

    /// <summary>
    /// Gets or sets the collection of supported indicator styles for this indicator.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public IReadOnlyList<Style> SupportedStyles { get; init; } = new[] { Style.Series };
}
