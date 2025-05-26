namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a streaming hub-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
[AttributeUsage(
    validOn: AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class StreamIndicatorAttribute(string id)
    : IndicatorAttribute(id, Style.Stream);
