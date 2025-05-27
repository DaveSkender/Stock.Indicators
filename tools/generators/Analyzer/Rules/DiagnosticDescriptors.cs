namespace Generators.Analyzer.Rules;

/// <summary>
/// Contains diagnostic descriptors for the catalog analyzer.
/// </summary>
internal static class DiagnosticDescriptors
{
    // Rule and descriptor categories
    private const string ValidationCategory = "Validation";

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
