namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a streaming hub-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <remarks>
/// <para>
/// Apply this attribute to static methods that implement stream-style indicators.
/// Stream indicators typically process values one at a time in a streaming fashion
/// and return a calculator object that maintains state.
/// </para>
/// <para>
/// Example usage:
/// <code>
/// [StreamIndicator("STOCH")]
/// public static StochCalculator GetStoch()
/// {
///     return new StochCalculator();
/// }
/// </code>
/// </para>
/// <para>
/// When this attribute is applied, the analyzer will validate that an explicitly defined
/// catalog listing exists for the indicator that matches the method signature.
/// </para>
/// </remarks>
[AttributeUsage(
    validOn: AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class StreamIndicatorAttribute(string id)
    : IndicatorAttribute(id, Style.Stream);
