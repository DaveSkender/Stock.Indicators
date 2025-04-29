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
        minValue: Enum.GetValues<T>().Cast<T>().Min(),
        maxValue: Enum.GetValues<T>().Cast<T>().Max()
    )
    where T : struct, Enum;
