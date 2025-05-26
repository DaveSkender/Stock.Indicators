namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for date type parameters used in catalog generation.
/// </summary>
/// <param name="displayName">The display name of the parameter.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamDateAttribute(
    string displayName
) : ParamAttribute<DateTime>(
    displayName: displayName,
    defaultValue: default,
    minValue: DateTime.MinValue,
    maxValue: DateTime.MaxValue
)
{
    /// <summary>
    /// Gets the TypeScript-friendly data type for the date.
    /// </summary>
    public string DataType { get; } = "Date";
}
