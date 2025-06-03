namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a buffer-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <remarks>
/// <para>
/// Apply this attribute to static methods that implement buffer-style indicators.
/// Buffer indicators maintain internal state and calculate based on a rolling window of values.
/// </para>
/// <para>
/// Example usage:
/// <code>
/// [BufferIndicator("ADL")]
/// public static AdlCalculator GetAdl(int lookbackPeriods)
/// {
///     return new AdlCalculator(lookbackPeriods);
/// }
/// </code>
/// </para>
/// <para>
/// When this attribute is applied, the analyzer will validate that an explicitly defined
/// catalog listing exists for the indicator that matches the method signature.
/// </para>
/// </remarks>
[AttributeUsage(
    validOn: AttributeTargets.Constructor,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class BufferIndicatorAttribute(string id)
    : IndicatorAttribute(id, Style.Buffer);
