namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for time-series indicator parameters used in catalog generation.
/// </summary>
/// <typeparam name="T">The interface type required for series elements.</typeparam>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="tooltipOverride">Optional custom template for parameter legend display.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamSeriesAttribute<T>(
    string displayName,
    string? tooltipOverride = null
) : ParamAttribute<IEnumerable<T>>(
    displayName: displayName,
    defaultValue: [],
    minValue: [],
    maxValue: [],
    tooltipOverride: tooltipOverride
) where T : IReusable
{
    /// <summary>
    /// Gets the TypeScript-friendly data type for the series.
    /// </summary>
    public string DataType { get; } = typeof(T).Name + "[]";
}
