// IndicatorAttributeInfo.cs
// Model for representing indicator attribute information
namespace Generators.Catalogger.Models;

/// <summary>
/// Represents information extracted from indicator attributes
/// </summary>
public class IndicatorAttributeInfo
{
    /// <summary>
    /// The ID of the indicator
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// The style of the indicator, if any
    /// </summary>
    public string? Style { get; }

    /// <summary>
    /// The name of the indicator
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// The description of the indicator
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// The URL for more information about the indicator
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// Creates a new instance of IndicatorAttributeInfo
    /// </summary>
    public IndicatorAttributeInfo(string? id, string? style)
    {
        Id = id;
        Style = style;
    }

    /// <summary>
    /// Creates a new instance of IndicatorAttributeInfo with all properties
    /// </summary>
    public IndicatorAttributeInfo(string? id, string? style, string? name, string? description, string? url)
    {
        Id = id;
        Style = style;
        Name = name;
        Description = description;
        Url = url;
    }
}
