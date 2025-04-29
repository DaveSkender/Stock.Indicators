namespace Skender.Stock.Indicators;

/// <summary>
/// Marks an indicator method to be excluded from catalog analysis warnings.
/// </summary>
/// <remarks>
/// Use this attribute on indicator methods that should be excluded from the IND001 analyzer warnings,
/// such as utility methods that technically match the pattern but are not main indicator entry points.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class ExcludeFromCatalogAttribute : Attribute { }
