namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator.
/// </summary>
/// <param name="id">Unique code of the indicator (e.g. "SMA")</param>
/// <remarks>
/// <para>
/// Apply this attribute to static methods that implement series-style indicators.
/// Series indicators typically process a series of values and return results for each input value.
/// </para>
/// <para>
/// Example usage:
/// <code>
/// [SeriesIndicator("EMA")]
/// public static IReadOnlyList&lt;EmaResult&gt; ToEma&lt;T&gt;(
///     this IReadOnlyList&lt;T&gt; source,
///     int lookbackPeriods)
///     where T : IReusable
/// {
///     // Implementation
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
internal sealed class SeriesIndicatorAttribute(string id)
    : IndicatorAttribute(id, Style.Series);
