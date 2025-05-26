namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a buffer-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
[AttributeUsage(
    validOn: AttributeTargets.Constructor,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class BufferIndicatorAttribute(string id)
    : IndicatorAttribute(id, Style.Buffer);
