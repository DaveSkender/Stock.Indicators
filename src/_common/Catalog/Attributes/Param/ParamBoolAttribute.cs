namespace Skender.Stock.Indicators;

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
    defaultValue: defaultValue,
    minValue: false,
    maxValue: true
);
