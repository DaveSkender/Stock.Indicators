namespace Skender.Stock.Indicators;

/// <summary>
/// Parameter attribute for Enum indicator parameters used in catalog generation.
/// </summary>
/// <inheritdoc cref="ParamAttribute{T}"/>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class ParamEnumAttribute<T>(
    string displayName,
    T defaultValue
) : ParamAttribute<T>(
        displayName: displayName,
        defaultValue: defaultValue,
        minValue: default, // Min/max will be determined by the generator
        maxValue: default  // which has access to all enum values
    )
    where T : struct, Enum;
