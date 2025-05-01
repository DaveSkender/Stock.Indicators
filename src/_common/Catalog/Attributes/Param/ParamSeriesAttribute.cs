namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for time-series indicator parameters used in catalog generation.
/// </summary>
/// <typeparam name="T">The interface type required for series elements.</typeparam>
/// <param name="displayName">The display name of the parameter.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamSeriesAttribute<T>(
    string displayName
) : ParamAttribute<IEnumerable<T>>(
    displayName: displayName,
    defaultValue: [],
    minValue: [],
    maxValue: []
) where T : IReusable
{
    /// <summary>
    /// Gets the TypeScript-friendly data type for the series.
    /// </summary>
    public string DataType { get; } = $"{typeof(T).Name}[]";
}
