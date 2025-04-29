namespace Skender.Stock.Indicators;

/// <summary>
/// Base class for parameter attributes used in catalog generation.
/// </summary>
/// <typeparam name="T">The type of the parameter value.</typeparam>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="defaultValue">The default value for the parameter.</param>
/// <param name="minValue">The minimum allowed value for the parameter.</param>
/// <param name="maxValue">The maximum allowed value for the parameter.</param>
internal abstract class ParamAttribute<T>(
   string displayName,
   T defaultValue,
   T minValue,
   T maxValue
) : Attribute
{
    /// <summary>
    /// Gets the display name of the parameter.
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>
    /// Gets the default value for the parameter.
    /// </summary>
    public T DefaultValue { get; } = defaultValue;

    /// <summary>
    /// Gets the minimum allowed value for the parameter.
    /// </summary>
    public T MinValue { get; } = minValue;

    /// <summary>
    /// Gets the maximum allowed value for the parameter.
    /// </summary>
    public T MaxValue { get; } = maxValue;
}
