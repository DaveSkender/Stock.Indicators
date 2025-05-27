namespace Generators.Analyzer.Rules;

/// <summary>
/// Analyzer that validates indicator catalog listings for the catalog system.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CatalogSystemAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [
            CatalogDiagnosticDescriptors.SID001_MissingListingDescriptor,
            CatalogDiagnosticDescriptors.SID002_MissingParameterDescriptor,
            CatalogDiagnosticDescriptors.SID003_ExtraneousParameterDescriptor,
            CatalogDiagnosticDescriptors.SID004_ParameterTypeMismatchDescriptor,
            CatalogDiagnosticDescriptors.SID005_MissingResultDescriptor,
        ];

    /// <summary>
    /// Initializes the analyzer.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register analyzer for class declarations
        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        // Apply catalog system rules
        CatalogRules.AnalyzeMissingListing(context, classDeclaration);
        CatalogRules.AnalyzeParametersConsistency(context, classDeclaration);
        CatalogRules.AnalyzeResultsConsistency(context, classDeclaration);
    }
}
