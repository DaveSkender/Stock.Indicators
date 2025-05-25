namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for date type parameters used in catalog generation.
/// </summary>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="defaultValue">The default value for the parameter (optional).</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamDateAttribute(
    string displayName,
    DateTime? defaultValue = null
) : ParamAttribute<DateTime>(
    displayName: displayName,
    defaultValue: defaultValue ?? default,
    minValue: DateTime.MinValue,
    maxValue: DateTime.MaxValue
)
{
    /// <summary>
    /// Gets the TypeScript-friendly data type for the date.
    /// </summary>
    public string DataType { get; } = "Date";
}
