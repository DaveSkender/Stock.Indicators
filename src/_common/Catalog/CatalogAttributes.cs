namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="name">Name of the indicator</param>
/// <param name="category">Category of the indicator</param>
/// <param name="chartType">Chart type of the indicator</param>
/// <param name="legendOverride">Optional custom legend format to override the automatic format</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class SeriesAttribute(
    string id,
    string name,
    Category category,
    ChartType chartType,
    string? legendOverride = null
) : CatalogAttribute(id, name, Style.Series, category, chartType, legendOverride)
{ }

/// <summary>
/// Classification attribute for a streaming hub-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="name">Name of the indicator</param>
/// <param name="category">Category of the indicator</param>
/// <param name="chartType">Chart type of the indicator</param>
/// <param name="legendOverride">Optional custom legend format to override the automatic format</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class StreamAttribute(
    string id,
    string name,
    Category category,
    ChartType chartType,
    string? legendOverride = null
) : CatalogAttribute(id, name, Style.Stream, category, chartType, legendOverride)
{ }

/// <summary>
/// Classification attribute for a buffer-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="name">Name of the indicator</param>
/// <param name="category">Category of the indicator</param>
/// <param name="chartType">Chart type of the indicator</param>
/// <param name="legendOverride">Optional custom legend format to override the automatic format</param>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class BufferAttribute(
    string id,
    string name,
    Category category,
    ChartType chartType,
    string? legendOverride = null
) : CatalogAttribute(id, name, Style.Buffer, category, chartType, legendOverride)
{ }

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
internal abstract class CatalogAttribute(
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

/// <summary>
/// Marks an indicator method to be excluded from catalog analysis warnings.
/// </summary>
/// <remarks>
/// Use this attribute on indicator methods that should be excluded from the IND001 analyzer warnings,
/// such as utility methods that technically match the pattern but are not main indicator entry points.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class ExcludeFromCatalogAttribute : Attribute { }

/// <summary>
/// Classification attribute for a series-style indicator
/// for catalog generation.
/// </summary>
/// <param name="displayName"></param>
/// <param name="minValue"></param>
/// <param name="maxValue"></param>
/// <param name="defaultValue"></param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamAttribute(
    string displayName,
    double minValue,
    double maxValue,
    double defaultValue
) : Attribute
{
    public string DisplayName { get; } = displayName;
    public double MinValue { get; } = minValue;
    public double MaxValue { get; } = maxValue;
    public double DefaultValue { get; } = defaultValue;
}

internal enum Style
{
    Series,
    Buffer,
    Stream
}

/// <summary>
/// Represents the category of an indicator.
/// </summary>
public enum Category
{
    /// <summary>
    /// Undefined category.
    /// </summary>
    Undefined,

    /// <summary>
    /// Indicators related to candlestick patterns.
    /// </summary>
    CandlestickPattern,

    /// <summary>
    /// Indicators related to moving averages.
    /// </summary>
    MovingAverage,

    /// <summary>
    /// Indicators that are oscillators.
    /// </summary>
    Oscillator,

    /// <summary>
    /// Indicators related to price channels.
    /// </summary>
    PriceChannel,

    /// <summary>
    /// Indicators that describe price characteristics.
    /// </summary>
    PriceCharacteristic,

    /// <summary>
    /// Indicators related to price patterns.
    /// </summary>
    PricePattern,

    /// <summary>
    /// Indicators that transform price data.
    /// </summary>
    PriceTransform,

    /// <summary>
    /// Indicators that analyze price trends.
    /// </summary>
    PriceTrend,

    /// <summary>
    /// Indicators related to stop and reverse strategies.
    /// </summary>
    StopAndReverse,

    /// <summary>
    /// Indicators based on volume data.
    /// </summary>
    VolumeBased,
}

/// <summary>
/// Represents the type of chart used for an indicator.
/// </summary>
public enum ChartType
{
    /// <summary>
    /// Undefined chart type.
    /// </summary>
    Undefined,

    /// <summary>
    /// Overlay chart type.
    /// </summary>
    Overlay,

    /// <summary>
    /// Oscillator chart type.
    /// </summary>
    Oscillator
}
