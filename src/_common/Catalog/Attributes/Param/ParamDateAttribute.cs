namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for date type parameters used in catalog generation.
/// </summary>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="tooltipOverride">Optional custom template for parameter legend display.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamDateAttribute(
    string displayName,
    string? tooltipOverride = null
) : ParamAttribute<DateTime>(
    displayName: displayName,
    defaultValue: default,
    minValue: DateTime.MinValue,
    maxValue: DateTime.MaxValue,
    tooltipOverride: tooltipOverride
)
{
    /// <summary>
    /// Gets the TypeScript-friendly data type for the date.
    /// </summary>
    public string DataType { get; } = "Date";
}
