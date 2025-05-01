namespace Generators.Analyzer.Rules;

/// <summary>
/// Contains diagnostic descriptors for the catalog analyzer.
/// </summary>
internal static class DiagnosticDescriptors
{
    // Rule IDs
    public const string SeriesDiagnosticId = "IND001";
    public const string StreamDiagnosticId = "IND002";
    public const string BufferDiagnosticId = "IND003";
    public const string MissingParamDiagnosticId = "IND100";
    public const string TypeMismatchDiagnosticId = "IND101";
    // Note: IND9xx are reserved generation runtime errors

    // Categories
    private const string Category = "Usage";

    // Descriptions
    private const string IndicatorStyleDescription = "Indicator methods should have the appropriate catalog attribute based on their style.";
    private const string ParamDescription = "Indicator method parameters should have the ParamAttribute applied.";
    private const string TypeMismatchDescription = "ParamAttribute's generic type parameter should match the parameter type it decorates.";

    // Series rule (IND001)
    private const string SeriesTitle = "Series indicator method missing Series attribute";
    private const string SeriesMessageFormat = "Series indicator method '{0}' must have the Series attribute";

    public static readonly DiagnosticDescriptor SeriesRule = new(
        SeriesDiagnosticId,
        SeriesTitle,
        SeriesMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IndicatorStyleDescription);

    // Stream rule (IND002)
    private const string StreamTitle = "Stream hub indicator method missing Stream attribute";
    private const string StreamMessageFormat = "Stream hub indicator method '{0}' must have the Stream attribute";

    public static readonly DiagnosticDescriptor StreamRule = new(
        StreamDiagnosticId,
        StreamTitle,
        StreamMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IndicatorStyleDescription);

    // Buffer rule (IND003)
    private const string BufferTitle = "Buffer indicator method missing Buffer attribute";
    private const string BufferMessageFormat = "Buffer indicator method '{0}' must have the Buffer attribute";

    public static readonly DiagnosticDescriptor BufferRule = new(
        BufferDiagnosticId,
        BufferTitle,
        BufferMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IndicatorStyleDescription);

    // Missing parameter attribute rule (IND100)
    private const string MissingParamTitle = "Missing ParamAttribute on indicator parameter";
    private const string MissingParamMessageFormat = "Parameter '{0}' in method '{1}' with IndicatorAttribute is missing a ParamAttribute";

    public static readonly DiagnosticDescriptor MissingParamRule = new(
        MissingParamDiagnosticId,
        MissingParamTitle,
        MissingParamMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: ParamDescription);

    // Type mismatch rule (IND101)
    private const string TypeMismatchTitle = "Type mismatch between attribute generic type and parameter type";
    private const string TypeMismatchMessageFormat = "ParamAttribute<{2}> should be adjusted to match parameter '{0}' type {3} in method '{1}'";

    public static readonly DiagnosticDescriptor TypeMismatchRule = new(
        TypeMismatchDiagnosticId,
        TypeMismatchTitle,
        TypeMismatchMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: TypeMismatchDescription);
}