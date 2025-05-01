namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for an indicator
/// for catalog generation.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="name">Name of the indicator</param>
/// <param name="style">Type of indicator (series, stream, or buffer</param>
/// <param name="category">Category of the indicator</param>
/// <param name="chartType">Chart type of the indicator</param>
/// <param name="legendOverride">Optional custom legend format to override the automatic format</param>
internal abstract class IndicatorAttribute(
    string id,
    string name,
    Style style,
    Category category,
    ChartType chartType,
    string? legendOverride = null
) : Attribute
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public Style Style { get; } = style;
    public Category Category { get; } = category;
    public ChartType ChartType { get; } = chartType;

    /// <summary>
    /// Gets the custom legend format that overrides the automatic format.
    /// If provided, this format will be used instead of the auto-generated format.
    /// </summary>
    public string? LegendOverride { get; } = legendOverride;

    /// <inheritdoc/>
    public override string ToString() => $"{Style}: {Name} ({Id})";
}
