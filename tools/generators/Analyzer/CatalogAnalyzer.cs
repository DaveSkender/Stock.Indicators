namespace Generators.Analyzer;

/// <summary>
/// Analyzer that checks if indicator methods have the
/// required style-specific catalog attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CatalogAnalyzer : DiagnosticAnalyzer
{
    // Define common utility method names to exclude
    private static readonly HashSet<string> ExcludedMethodNames
        = new(StringComparer.OrdinalIgnoreCase) {
            "Condense",
            "RemoveWarmupPeriods"
          };

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [
            DiagnosticDescriptors.IND001_MissingSeriesAttributeDescriptor,
            DiagnosticDescriptors.IND002_MissingStreamAttributeDescriptor,
            DiagnosticDescriptors.IND003_MissingBufferAttributeDescriptor,
            DiagnosticDescriptors.IND101_MissingParamAttributeDescriptor,
            DiagnosticDescriptors.IND102_ParamTypeMismatchDescriptor
        ];

    /// <summary>
    /// Initializes the analyzer.
    /// </summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration)
        {
            return;
        }

        // Check if this is a public static method
        if (!IsPublicStaticMethod(methodDeclaration))
        {
            return;
        }

        // Get the method symbol
        IMethodSymbol? methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
        if (methodSymbol == null)
        {
            return;
        }

        // Skip if the method has attributes that exclude it from analysis
        if (AnalyzerUtils.HasObsoleteAttribute(methodSymbol) || AnalyzerUtils.HasExcludeFromCatalogAttribute(methodSymbol))
        {
            return;
        }

        // Skip common utility methods
        if (IsCommonUtilityMethod(methodSymbol))
        {
            return;
        }

        // Skip extension methods that don't operate on the main indicator types
        if (IsExtensionMethod(methodSymbol) && !IsMainIndicatorExtension(methodSymbol))
        {
            return;
        }

        // Check for any IndicatorAttribute first to validate ParamAttributes
        bool hasCatalogAttribute = HasAnyCatalogAttribute(methodSymbol);

        // Apply style rules (IND001-003)
        IndicatorStyleRules.Analyze(context, methodSymbol, methodDeclaration);

        // If the method has a catalog attribute, check parameters for ParamAttribute (IND101-101)
        if (hasCatalogAttribute)
        {
            ParamAttributeRules.Analyze(context, methodSymbol);
        }
    }

    /// <summary>
    /// Determines if the method is a public static method.
    /// </summary>
    private static bool IsPublicStaticMethod(MethodDeclarationSyntax method) =>
        method.Modifiers.Any(SyntaxKind.PublicKeyword) &&
        method.Modifiers.Any(SyntaxKind.StaticKeyword);

    /// <summary>
    /// Determines if the method is a common utility method.
    /// </summary>
    private static bool IsCommonUtilityMethod(IMethodSymbol methodSymbol) =>
        // Check if the method name is in our list of known utility methods
        ExcludedMethodNames.Contains(methodSymbol.Name);

    /// <summary>
    /// Determines if the method is an extension method.
    /// </summary>
    private static bool IsExtensionMethod(IMethodSymbol methodSymbol) =>
        methodSymbol.IsExtensionMethod;

    /// <summary>
    /// Determines if the method is a main indicator extension.
    /// </summary>
    private static bool IsMainIndicatorExtension(IMethodSymbol methodSymbol) =>
        // This is a heuristic to identify the main indicator extension methods
        // These typically start with "To" and convert data to indicator results
        methodSymbol.Name.StartsWith("To") || methodSymbol.Name.StartsWith("Get");

    /// <summary>
    /// Determines if the method has any catalog-related attribute.
    /// </summary>
    private static bool HasAnyCatalogAttribute(IMethodSymbol methodSymbol)
    {
        foreach (AttributeData attribute in methodSymbol.GetAttributes())
        {
            if (AnalyzerUtils.IsIndicatorAttributeOrDerived(attribute.AttributeClass))
            {
                return true;
            }
        }

        return false;
    }
}
