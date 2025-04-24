using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Indicators.Catalog.Generator;

/// <summary>
/// Analyzer that checks if indicator methods have the
/// required style-specific catalog attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CatalogAnalyzer : DiagnosticAnalyzer
{
    public const string SeriesDiagnosticId = "IND001";
    public const string StreamDiagnosticId = "IND002";
    public const string BufferDiagnosticId = "IND003";

    private const string SeriesTitle = "Series indicator method missing Series attribute";
    private const string StreamTitle = "Stream hub indicator method missing Stream attribute";
    private const string BufferTitle = "Buffer indicator method missing Buffer attribute";

    private const string SeriesMessageFormat = "Series indicator method '{0}' must have the Series attribute";
    private const string StreamMessageFormat = "Stream hub indicator method '{0}' must have the Stream attribute";
    private const string BufferMessageFormat = "Buffer indicator method '{0}' must have the Buffer attribute";

    private const string Description = "Indicator methods should have the appropriate catalog attribute based on their style.";
    private const string Category = "Usage";

    // Define constant for "not an indicator"
    private const int StyleNone = -1;

    // Define common utility method names to exclude
    private static readonly HashSet<string> ExcludedMethodNames
        = new(StringComparer.OrdinalIgnoreCase) {
            "Condense",
            "RemoveWarmupPeriods"
          };

    private static readonly DiagnosticDescriptor SeriesRule = new(
        SeriesDiagnosticId,
        SeriesTitle,
        SeriesMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    private static readonly DiagnosticDescriptor StreamRule = new(
        StreamDiagnosticId,
        StreamTitle,
        StreamMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    private static readonly DiagnosticDescriptor BufferRule = new(
        BufferDiagnosticId,
        BufferTitle,
        BufferMessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [SeriesRule, StreamRule, BufferRule];

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
        if (HasObsoleteAttribute(methodSymbol) || HasExcludeFromCatalogAttribute(methodSymbol))
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

        // Determine the indicator style based on the return type
        int styleValue = DetermineIndicatorStyle(methodSymbol, context.Compilation);

        // If not an indicator method, skip further analysis
        if (styleValue == StyleNone)
        {
            return;
        }

        // Convert to Skender.Stock.Indicators.Style enum value for comparison
        // Style enum: Series = 0, Buffer = 1, Stream = 2
        if (styleValue == 0 && !HasAttributeOfType(methodDeclaration, "SeriesAttribute", context.SemanticModel))
        {
            // Series style indicator missing SeriesAttribute
            context.ReportDiagnostic(Diagnostic.Create(
                SeriesRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text));
        }
        else if (styleValue == 2 && !HasAttributeOfType(methodDeclaration, "StreamAttribute", context.SemanticModel))
        {
            // Stream style indicator missing StreamAttribute
            context.ReportDiagnostic(Diagnostic.Create(
                StreamRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text));
        }
        else if (styleValue == 1 && !HasAttributeOfType(methodDeclaration, "BufferAttribute", context.SemanticModel))
        {
            // Buffer style indicator missing BufferAttribute
            context.ReportDiagnostic(Diagnostic.Create(
                BufferRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text));
        }
    }

    /// <summary>
    /// Determines the indicator style based on the return type.
    /// </summary>
    /// <param name="methodSymbol">The method symbol to analyze.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>
    /// The Style enum value (0=Series, 1=Buffer, 2=Stream), or StyleNone (-1) if not an indicator.
    /// </returns>
    /// <remarks>
    /// We need a StyleNone value here to distinguish between methods that are not indicators
    /// and methods that are indicators but we couldn't determine the style.
    /// </remarks>
    private static int DetermineIndicatorStyle(IMethodSymbol methodSymbol, Compilation compilation)
    {
        // Get the return type
        ITypeSymbol returnType = methodSymbol.ReturnType;

        // Check if the method returns a Stream type
        if (IsStreamType(returnType, compilation))
        {
            return 2; // Style.Stream
        }

        // Check if the method returns a Buffer type
        if (IsBufferType(returnType, compilation))
        {
            return 1; // Style.Buffer
        }

        // Check if the method returns a collection of result objects
        if (IsSeriesType(returnType, compilation))
        {
            return 0; // Style.Series
        }

        // Not an indicator method or couldn't determine the style
        return StyleNone;
    }

    private static bool IsCommonUtilityMethod(IMethodSymbol methodSymbol) =>
        // Check if the method name is in our list of known utility methods
        ExcludedMethodNames.Contains(methodSymbol.Name);

    private static bool IsExtensionMethod(IMethodSymbol methodSymbol) => methodSymbol.IsExtensionMethod;

    private static bool IsMainIndicatorExtension(IMethodSymbol methodSymbol) =>
        // This is a heuristic to identify the main indicator extension methods
        // These typically start with "To" and convert data to indicator results
        methodSymbol.Name.StartsWith("To") || methodSymbol.Name.StartsWith("Get");

    private static bool IsStreamType(ITypeSymbol type, Compilation compilation)
    {
        // Check if the type implements IStreamHub
        INamedTypeSymbol? streamHubInterface = compilation.GetTypeByMetadataName("Skender.Stock.Indicators.IStreamHub`2");
        if (streamHubInterface != null)
        {
            foreach (INamedTypeSymbol iface in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(iface.OriginalDefinition, streamHubInterface))
                {
                    return true;
                }
            }
        }

        // Fallback check for Hub naming convention
        return type.Name.EndsWith("Hub");
    }

    private static bool IsBufferType(ITypeSymbol type, Compilation compilation)
    {
        // Get the IBufferQuote interface symbol
        INamedTypeSymbol? bufferQuoteInterface = compilation.GetTypeByMetadataName("Skender.Stock.Indicators.IBufferQuote");

        if (bufferQuoteInterface != null)
        {
            // Check if the type implements IBufferQuote
            if (type.AllInterfaces.Any(i =>
                SymbolEqualityComparer.Default.Equals(i, bufferQuoteInterface)))
            {
                return true;
            }
        }

        // Check naming patterns for Buffer types
        return type.Name.EndsWith("Buffer") ||
               (type.Name.EndsWith("List") && type.AllInterfaces.Any(i => i.Name.Contains("Buffer")));
    }

    private static bool IsSeriesType(ITypeSymbol type, Compilation compilation)
    {
        // If it's a collection type
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            // Check if it's a collection type (IEnumerable<T>, IReadOnlyList<T>, etc.)
            string typeName = namedType.ToString();
            bool isCollection = typeName.Contains("IEnumerable") ||
                                typeName.Contains("IReadOnlyList") ||
                                typeName.Contains("List");

            if (!isCollection)
            {
                return false;
            }

            // Check if the collection element type ends with "Result"
            if (namedType.TypeArguments.Length > 0)
            {
                ITypeSymbol elementType = namedType.TypeArguments[0];

                if (elementType.Name.EndsWith("Result"))
                {
                    // Get the IIndicatorResult interface symbol
                    INamedTypeSymbol? indicatorResultInterface =
                        compilation.GetTypeByMetadataName("Skender.Stock.Indicators.IIndicatorResult");

                    // If we can find the interface, check if the element type implements it
                    if (indicatorResultInterface != null)
                    {
                        return elementType.AllInterfaces.Any(i =>
                            SymbolEqualityComparer.Default.Equals(i, indicatorResultInterface));
                    }

                    // Fallback to just checking the Result suffix
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsPublicStaticMethod(MethodDeclarationSyntax method) =>
        method.Modifiers.Any(SyntaxKind.PublicKeyword) &&
        method.Modifiers.Any(SyntaxKind.StaticKeyword);

    private static bool HasObsoleteAttribute(IMethodSymbol methodSymbol)
    {
        // Check if the method has the ObsoleteAttribute
        foreach (AttributeData attribute in methodSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Name == "ObsoleteAttribute" ||
                attribute.AttributeClass?.ToString() == "System.ObsoleteAttribute")
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasExcludeFromCatalogAttribute(IMethodSymbol methodSymbol)
    {
        // Check if the method has the ExcludeFromCatalogAttribute
        foreach (AttributeData attribute in methodSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Name == "ExcludeFromCatalogAttribute" ||
                attribute.AttributeClass?.ToString() == "Skender.Stock.Indicators.ExcludeFromCatalogAttribute")
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAttributeOfType(MethodDeclarationSyntax method, string attributeName, SemanticModel semanticModel)
    {
        foreach (AttributeListSyntax attributeList in method.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                ISymbol? attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol;
                if (attributeSymbol?.ContainingType?.Name == attributeName)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
