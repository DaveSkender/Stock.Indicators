namespace Skender.Stock.Indicators;

/// <summary>
/// Classification attribute for a series-style indicator
/// for catalog generation.
/// </summary>
/// <param name="displayName"></param>
/// <param name="minValue"></param>
/// <param name="maxValue"></param>
/// <param name="defaultValue"></param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamAttribute(
    string displayName,
    double minValue,
    double maxValue,
    double defaultValue
) : Attribute
{
    public string DisplayName { get; } = displayName;
    public double MinValue { get; } = minValue;
    public double MaxValue { get; } = maxValue;
    public double DefaultValue { get; } = defaultValue;
}
