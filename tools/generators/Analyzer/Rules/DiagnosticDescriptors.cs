namespace Generators.Analyzer.Rules;

/// <summary>
/// Contains diagnostic descriptors for the catalog analyzer.
/// </summary>
internal static class DiagnosticDescriptors
{
    // Rule and descriptor categories
    private const string AttributedCategory = "Attrubuted";
    private const string ValidationCategory = "Validation";

    // IND0xx: Common values
    private const string IND00x_Description
        = "Indicator methods should have the appropriate catalog attribute based on their style.";

    // IND001: Series indicator type attribute is missing
    private const string IND001_RuleId = "IND001";
    private const string IND001_Title = "Series indicator method missing Series attribute";
    private const string IND001_MessageFormat = "Series indicator method '{0}' must have the Series attribute";

    public static readonly DiagnosticDescriptor IND001_MissingSeriesAttributeDescriptor = new(
        id: IND001_RuleId,
        title: IND001_Title,
        messageFormat: IND001_MessageFormat,
        category: AttributedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IND00x_Description);

    // IND002: StreamHub indicator type attribute is missing
    private const string IND002_RuleId = "IND002";
    private const string IND002_Title = "Stream hub indicator method missing Stream attribute";
    private const string IND002_MessageFormat = "Stream hub indicator method '{0}' must have the Stream attribute";

    public static readonly DiagnosticDescriptor IND002_MissingStreamAttributeDescriptor = new(
        id: IND002_RuleId,
        title: IND002_Title,
        messageFormat: IND002_MessageFormat,
        category: AttributedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IND00x_Description);

    // IND003: BufferList indicator type attribute is missing
    private const string IND003_RuleId = "IND003";
    private const string IND003_Title = "Buffer indicator method missing Buffer attribute";
    private const string IND003_MessageFormat = "Buffer indicator method '{0}' must have the Buffer attribute";

    public static readonly DiagnosticDescriptor IND003_MissingBufferAttributeDescriptor = new(
        id: IND003_RuleId,
        title: IND003_Title,
        messageFormat: IND003_MessageFormat,
        category: AttributedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IND00x_Description);

    // IND101: Missing parameter attribute
    private const string IND101_RuleId = "IND101";
    private const string IND101_Title = "Missing ParamAttribute on indicator parameter";
    private const string IND101_MessageFormat = "Parameter '{0}' in method '{1}' with IndicatorAttribute is missing a ParamAttribute";
    private const string IND101_Description = "Indicator method parameters should have the ParamAttribute applied.";

    public static readonly DiagnosticDescriptor IND101_MissingParamAttributeDescriptor = new(
        id: IND101_RuleId,
        title: IND101_Title,
        messageFormat: IND101_MessageFormat,
        category: AttributedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IND101_Description);

    // IND102: Parameter attribution type mismatch
    private const string IND102_RuleId = "IND102";
    private const string IND102_Title = "Type mismatch between attribute generic type and parameter type";
    private const string IND102_MessageFormat = "ParamAttribute<{2}> should be adjusted to match parameter '{0}' type {3} in method '{1}'";
    private const string IND102_Description = "ParamAttribute's generic type parameter should match the parameter type it decorates.";

    public static readonly DiagnosticDescriptor IND102_ParamTypeMismatchDescriptor = new(
        id: IND102_RuleId,
        title: IND102_Title,
        messageFormat: IND102_MessageFormat,
        category: AttributedCategory,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: IND102_Description);

    // IND901: Duplicate UIIDs found in catalog listing
    private const string IND901_RuleId = "IND901";
    private const string IND901_Title = "Duplicate UIIDs detected";
    private const string IND901_MessageFormat = "The following UIIDs are used more than once: {0}";
    private const string IND901_Description = "Indicator UIIDs must be unique across all indicators.";

    public static readonly DiagnosticDescriptor IND901_DuplicateUiidFoundDescriptor = new(
        id: IND901_RuleId,
        title: IND901_Title,
        messageFormat: IND901_MessageFormat,
        category: ValidationCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: IND901_Description);

    // IND902: Invalid default value rule
    private const string IND902_RuleId = "IND902";
    private const string IND902_Title = "Default value must be between min/max value range";
    private const string IND902_MessageFormat = "ParamNum values [{0}, {1}, {2}] are invalid. Default is out of min/max range.";
    private const string IND902_Description = "Parameter default value must be within the specified min/max range.";

    public static readonly DiagnosticDescriptor IND902_InvalidDefaultValueDescriptor = new(
        id: IND902_RuleId,
        title: IND902_Title,
        messageFormat: IND902_MessageFormat,
        category: ValidationCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: IND902_Description);
}
