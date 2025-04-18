namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
/// <param name="Category">Category of the indicator</param>
/// <param name="ChartType">Chart type of the indicator</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class SeriesAttribute(
    string Id,
    string Name,
    Category Category,
    ChartType ChartType
) : IndicatorAttribute(Id, Name, Style.Series, Category, ChartType)
{ }

/// <summary>
/// Classification attribute for a streaming hub-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
/// <param name="Category">Category of the indicator</param>
/// <param name="ChartType">Chart type of the indicator</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class StreamHubAttribute(
    string Id,
    string Name,
    Category Category,
    ChartType ChartType
) : IndicatorAttribute(Id, Name, Style.Stream, Category, ChartType)
{ }

/// <summary>
/// Classification attribute for a buffer-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
/// <param name="Category">Category of the indicator</param>
/// <param name="ChartType">Chart type of the indicator</param>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class BufferAttribute(
    string Id,
    string Name,
    Category Category,
    ChartType ChartType
) : IndicatorAttribute(Id, Name, Style.Buffer, Category, ChartType)
{ }

/// <summary>
/// Classification attribute for an indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
/// <param name="Type">Type of indicator (series, stream, or buffer</param>
/// <param name="Category">Category of the indicator</param>
/// <param name="ChartType">Chart type of the indicator</param>
internal abstract class IndicatorAttribute(
    string Id,
    string Name,
    Style Type,
    Category Category,
    ChartType ChartType
) : Attribute
{
    public string Id { get; } = Id;
    public string Name { get; } = Name;
    public Style Style { get; } = Type;
    public Category Category { get; } = Category;
    public ChartType ChartType { get; } = ChartType;

    /// <inheritdoc/>
    public override string ToString() => $"{Style}: {Name} ({Id})";
}

internal enum Style
{
    Series,
    Buffer,
    Stream
}

internal enum Category
{
    MovingAverage,
    PriceTrend,
    VolumeBased,
    Oscillator,
    Pattern,
    PriceChannel,
    Generated
}

internal enum ChartType
{
    Overlay,
    Oscillator,
    Indicator
}
