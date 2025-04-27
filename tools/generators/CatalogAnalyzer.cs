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
    // Note: IND004 is reserved for the duplicate UIID validation in the CatalogGenerator
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
            // Check if it's a collection type
            string typeName = namedType.ToString();
            bool isCollection = typeName.Contains("IEnumerable") ||
                                typeName.Contains("IReadOnlyList") ||
                                typeName.Contains("List");

            if (!isCollection || namedType.TypeArguments.Length == 0)
            {
                return false;
            }

            // Get the collection element type
            ITypeSymbol elementType = namedType.TypeArguments[0];

            // Check if the element type implements ISeries interface (directly or indirectly)
            INamedTypeSymbol? iSeriesInterface = compilation.GetTypeByMetadataName("Skender.Stock.Indicators.ISeries");
            if (iSeriesInterface != null)
            {
                // Check if element type implements ISeries (which includes implementing IReusable)
                return elementType.AllInterfaces.Any(i =>
                    SymbolEqualityComparer.Default.Equals(i, iSeriesInterface));
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
        // Get method symbol to access attribute data properly
        IMethodSymbol? methodSymbol = semanticModel.GetDeclaredSymbol(method);
        if (methodSymbol == null)
        {
            return false;
        }

        // Extract the style from attribute name (remove "Attribute" suffix if present)
        string styleToCheck = attributeName.Replace("Attribute", string.Empty);

        // Check all attributes on the method
        foreach (AttributeData attribute in methodSymbol.GetAttributes())
        {
            INamedTypeSymbol? attributeClass = attribute.AttributeClass;

            // Direct match on attribute name
            if (attributeClass?.Name == attributeName ||
                attributeClass?.Name == styleToCheck ||
                attributeClass?.ToString() == $"Skender.Stock.Indicators.{attributeName}")
            {
                return true;
            }

            // Check if derived from CatalogAttribute and look at Style property
            if (IsCatalogAttributeOrDerived(attributeClass))
            {
                // Check named arguments for Style property
                foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
                {
                    if (string.Equals(namedArg.Key, "Style", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(namedArg.Value.Value?.ToString(), styleToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                // Check constructor arguments for Style value
                foreach (TypedConstant arg in attribute.ConstructorArguments)
                {
                    if (string.Equals(arg.Value?.ToString(), styleToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the given type is CatalogAttribute or derived from it.
    /// </summary>
    private static bool IsCatalogAttributeOrDerived(INamedTypeSymbol? symbol)
    {
        if (symbol == null)
        {
            return false;
        }

        // Check if this is CatalogAttribute
        if (symbol.Name == "CatalogAttribute" ||
            symbol.ToString() == "Skender.Stock.Indicators.CatalogAttribute")
        {
            return true;
        }

        // Check base type recursively
        return symbol.BaseType != null && IsCatalogAttributeOrDerived(symbol.BaseType);
    }
}
