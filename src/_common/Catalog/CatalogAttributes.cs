using System.Globalization;

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
/// Parameter attribute for indicator parameters
/// for catalog generation.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamAttribute : Attribute
{
    /// <summary>
    /// Constructor for numeric (double) parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="minValue">Minimum allowed value</param>
    /// <param name="maxValue">Maximum allowed value</param>
    /// <param name="defaultValue">Default value</param>
    public ParamAttribute(
        string displayName,
        double minValue,
        double maxValue,
        double defaultValue)
    {
        DisplayName = displayName;
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultValue = defaultValue;
        DataType = "double";
    }

    /// <summary>
    /// Constructor for numeric (decimal) parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="minValue">Minimum allowed value</param>
    /// <param name="maxValue">Maximum allowed value</param>
    /// <param name="defaultValue">Default value</param>
    public ParamAttribute(
        string displayName,
        decimal minValue,
        decimal maxValue,
        decimal defaultValue)
    {
        DisplayName = displayName;
        MinValue = (double)minValue;
        MaxValue = (double)maxValue;
        DefaultValue = (double)defaultValue;
        DataType = "decimal";
    }

    /// <summary>
    /// Constructor for integer parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="minValue">Minimum allowed value</param>
    /// <param name="maxValue">Maximum allowed value</param>
    /// <param name="defaultValue">Default value</param>
    public ParamAttribute(
        string displayName,
        int minValue,
        int maxValue,
        int defaultValue)
    {
        DisplayName = displayName;
        MinValue = minValue;
        MaxValue = maxValue;
        DefaultValue = defaultValue;
        DataType = "int";
    }

    /// <summary>
    /// Constructor for boolean parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="defaultValue">Default value</param>
    public ParamAttribute(
        string displayName,
        bool defaultValue)
    {
        DisplayName = displayName;
        MinValue = 0;
        MaxValue = 1;
        DefaultValue = defaultValue ? 0 : 1;
        DataType = "boolean";
    }

    /// <summary>
    /// Constructor for PeriodSize enum parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="defaultValue">Default enum value</param>
    public ParamAttribute(
        string displayName,
        PeriodSize defaultValue)
    {
        DisplayName = displayName;
        MinValue = 0;
        MaxValue = Enum.GetValues<PeriodSize>().Cast<int>().Max();
        DefaultValue = Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture);
        DataType = "enum";
        EnumType = typeof(PeriodSize).Name;
    }

    /// <summary>
    /// Constructor for PivotPointType enum parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="defaultValue">Default enum value</param>
    public ParamAttribute(
        string displayName,
        PivotPointType defaultValue)
    {
        DisplayName = displayName;
        MinValue = 0;
        MaxValue = Enum.GetValues<PivotPointType>().Cast<int>().Max();
        DefaultValue = Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture);
        DataType = "enum";
        EnumType = typeof(PivotPointType).Name;
    }

    /// <summary>
    /// Constructor for EndType enum parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="defaultValue">Default enum value</param>
    public ParamAttribute(
        string displayName,
        EndType defaultValue)
    {
        DisplayName = displayName;
        MinValue = 0;
        MaxValue = Enum.GetValues<EndType>().Cast<int>().Max();
        DefaultValue = Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture);
        DataType = "enum";
        EnumType = typeof(EndType).Name;
    }

    /// <summary>
    /// Constructor for Direction enum parameters
    /// </summary>
    /// <param name="displayName">Display name for the parameter</param>
    /// <param name="defaultValue">Default enum value</param>
    public ParamAttribute(
        string displayName,
        Direction defaultValue)
    {
        DisplayName = displayName;
        MinValue = 0;
        MaxValue = Enum.GetValues<Direction>().Cast<int>().Max(); ;
        DefaultValue = Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture);
        DataType = "enum";
        EnumType = typeof(Direction).Name;
    }

    public string DisplayName { get; }
    public double MinValue { get; }
    public double MaxValue { get; }
    public double DefaultValue { get; }
    public string DataType { get; }
    public string? EnumType { get; }
}
