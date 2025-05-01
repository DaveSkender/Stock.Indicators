namespace Generators.Analyzer.Rules;

/// <summary>
/// Helper class for reporting diagnostic errors.
/// </summary>
internal static class DiagnosticsHelper
{
    internal static void ReportIND901_DuplicateListings(
        SourceProductionContext context,
        List<string> duplicateUIIDs) => context.ReportDiagnostic(
            diagnostic: Diagnostic.Create(
                descriptor: new DiagnosticDescriptor(
                    id: "IND901",
                    title: "Duplicate UIIDs detected",
                    messageFormat: "The following UIIDs are used more than once: {0}",
                    category: "Catalog",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location: Location.None,
                messageArgs: string.Join(", ", duplicateUIIDs)));

    internal static void ReportIND902_InvalidDefaultValue(
        SourceProductionContext context,
        double? defaultValue,
        double? minValue,
        double? maxValue) => context.ReportDiagnostic(
            diagnostic: Diagnostic.Create(
                descriptor: new DiagnosticDescriptor(
                    id: "IND902",
                    title: "Default value must be between min/max value range.",
                    messageFormat: "ParamNum values [{0}, {1}, {2}] are invalid. Default is out of min/max range.",
                    category: "Catalog",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location: Location.None,
                messageArgs: [defaultValue, minValue, maxValue]));
}
