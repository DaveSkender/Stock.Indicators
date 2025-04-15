namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class SeriesAttribute(
    string Id,
    string Name
) : IndicatorAttribute(Id, Name, IndicatorType.Series)
{ }

/// <summary>
/// Classification attribute for a streaming hub-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class StreamHubAttribute(
    string Id,
    string Name
) : IndicatorAttribute(Id, Name, IndicatorType.Stream)
{ }

/// <summary>
/// Classification attribute for a buffer-style indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
internal sealed class BufferAttribute(
    string Id,
    string Name
) : IndicatorAttribute(Id, Name, IndicatorType.Buffer)
{ }

/// <summary>
/// Classification attribute for an indicator.
/// </summary>
/// <param name="Id">Unique code of the indicator (e.g. "SMA")</param>
/// <param name="Name">Name of the indicator</param>
/// <param name="Type">Type of indicator (series, stream, or buffer</param>
internal abstract class IndicatorAttribute(
    string Id,
    string Name,
    IndicatorType Type
) : Attribute
{
    public string Id { get; } = Id;
    public string Name { get; } = Name;
    public IndicatorType Type { get; } = Type;

    /// <inheritdoc/>
    public override string ToString() => $"{Type}: {Name} ({Id})";
}

internal enum IndicatorType
{
    Series,
    Buffer,
    Stream
}
