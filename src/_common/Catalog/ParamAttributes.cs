using System.Numerics;

namespace Skender.Stock.Indicators;

/// <summary>
/// Base class for parameter attributes used in catalog generation.
/// </summary>
/// <typeparam name="T">The type of the parameter value.</typeparam>
/// <param name="displayName">The display name of the parameter.</param>
/// <param name="minValue">The minimum allowed value for the parameter.</param>
/// <param name="maxValue">The maximum allowed value for the parameter.</param>
/// <param name="defaultValue">The default value for the parameter.</param>
internal abstract class ParamAttribute<T>(
   string displayName,
   T minValue,
   T maxValue,
   T defaultValue
) : Attribute
{
    /// <summary>
    /// Gets the display name of the parameter.
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>
    /// Gets the minimum allowed value for the parameter.
    /// </summary>
    public T MinValue { get; } = minValue;

    /// <summary>
    /// Gets the maximum allowed value for the parameter.
    /// </summary>
    public T MaxValue { get; } = maxValue;

    /// <summary>
    /// Gets the default value for the parameter.
    /// </summary>
    public T DefaultValue { get; } = defaultValue;
}

/// <summary>
/// Parameter attribute for numeric indicator parameters used in catalog generation.
/// </summary>
/// <typeparam name="T">A numeric type that implements <see cref="INumber{TSelf}"/>.</typeparam>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamNumAttribute<T>
    : ParamAttribute<T>
    where T : INumber<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParamNumAttribute{T}"/> class with double values.
    /// </summary>
    /// <inheritdoc cref="ParamAttribute{T}"/>
    internal ParamNumAttribute(
        string displayName,
        double minValue,
        double maxValue,
        double defaultValue
    ) : base(
        displayName,
        T.CreateChecked(minValue),
        T.CreateChecked(maxValue),
        T.CreateChecked(defaultValue)
    )
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamNumAttribute{T}"/> class with decimal values.
    /// </summary>
    /// <inheritdoc cref="ParamAttribute{T}"/>
    internal ParamNumAttribute(
        string displayName,
        decimal minValue,
        decimal maxValue,
        decimal defaultValue
    ) : base(
        displayName,
        T.CreateChecked(minValue),
        T.CreateChecked(maxValue),
        T.CreateChecked(defaultValue)
    )
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamNumAttribute{T}"/> class with integer values.
    /// </summary>
    /// <inheritdoc cref="ParamAttribute{T}"/>
    internal ParamNumAttribute(
        string displayName,
        int minValue,
        int maxValue,
        int defaultValue
    ) : base(
        displayName,
        T.CreateChecked(minValue),
        T.CreateChecked(maxValue),
        T.CreateChecked(defaultValue)
    )
    { }
}

/// <summary>
/// Parameter attribute for Boolean indicator parameters used in catalog generation.
/// </summary>
/// <inheritdoc cref="ParamAttribute{T}"/>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamBoolAttribute(
    string displayName,
    bool defaultValue
) : ParamAttribute<bool>(
    displayName: displayName,
    minValue: false,
    maxValue: true,
    defaultValue: defaultValue
);

/// <summary>
/// Parameter attribute for Enum indicator parameters used in catalog generation.
/// </summary>
/// <inheritdoc cref="ParamAttribute{T}"/>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamEnumAttribute<T>(
    string displayName,
    T defaultValue
) : ParamAttribute<T>(
        displayName,
        minValue: Enum.GetValues<T>().Cast<T>().Min(),
        maxValue: Enum.GetValues<T>().Cast<T>().Max(),
        defaultValue: defaultValue
    )
    where T : struct, Enum;
