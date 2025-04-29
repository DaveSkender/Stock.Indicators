using System.Numerics;

namespace Skender.Stock.Indicators;

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
        double defaultValue,
        double minValue,
        double maxValue
    ) : base(
        displayName: displayName,
        defaultValue: T.CreateChecked(defaultValue),
        minValue: T.CreateChecked(minValue),
        maxValue: T.CreateChecked(maxValue)
    )
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamNumAttribute{T}"/> class with decimal values.
    /// </summary>
    /// <inheritdoc cref="ParamAttribute{T}"/>
    internal ParamNumAttribute(
        string displayName,
        decimal defaultValue,
        decimal minValue,
        decimal maxValue
    ) : base(
        displayName: displayName,
        defaultValue: T.CreateChecked(defaultValue),
        minValue: T.CreateChecked(minValue),
        maxValue: T.CreateChecked(maxValue)
    )
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParamNumAttribute{T}"/> class with integer values.
    /// </summary>
    /// <inheritdoc cref="ParamAttribute{T}"/>
    internal ParamNumAttribute(
        string displayName,
        int defaultValue,
        int minValue,
        int maxValue
    ) : base(
        displayName: displayName,
        defaultValue: T.CreateChecked(defaultValue),
        minValue: T.CreateChecked(minValue),
        maxValue: T.CreateChecked(maxValue)
    )
    { }
}
