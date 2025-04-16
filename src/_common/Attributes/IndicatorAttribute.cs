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
    IndicatorCategory Category,
    IndicatorChartType ChartType
) : IndicatorAttribute(Id, Name, IndicatorType.Series, Category, ChartType)
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
    IndicatorCategory Category,
    IndicatorChartType ChartType
) : IndicatorAttribute(Id, Name, IndicatorType.Stream, Category, ChartType)
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
    IndicatorCategory Category,
    IndicatorChartType ChartType
) : IndicatorAttribute(Id, Name, IndicatorType.Buffer, Category, ChartType)
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
    IndicatorType Type,
    IndicatorCategory Category,
    IndicatorChartType ChartType
) : Attribute
{
    public string Id { get; } = Id;
    public string Name { get; } = Name;
    public IndicatorType Type { get; } = Type;
    public IndicatorCategory Category { get; } = Category;
    public IndicatorChartType ChartType { get; } = ChartType;

    /// <inheritdoc/>
    public override string ToString() => $"{Type}: {Name} ({Id})";
}

internal enum IndicatorType
{
    Series,
    Buffer,
    Stream
}

internal enum IndicatorCategory
{
    MovingAverage,
    PriceTrend,
    VolumeBased,
    Oscillator,
    Pattern,
    PriceChannel,
    Generated
}

internal enum IndicatorChartType
{
    Overlay,
    Oscillator,
    Indicator
}
