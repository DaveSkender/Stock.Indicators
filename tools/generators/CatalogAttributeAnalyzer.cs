using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Stock.Indicators.Generator;

/// <summary>
/// Analyzer that checks if indicator public instantiation methods have the required IndicatorAttribute-derived attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CatalogAttributeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "IND001";
    private const string Title = "Indicator method missing required attribute";
    private const string MessageFormat = "Indicator method '{0}' must have one of the derived attribute types of 'CatalogAttribute'";
    private const string Description = "All indicator public instantiation methods should have one of the derived attribute types of IndicatorAttribute abstract class.";
    private const string Category = "Usage";

    // Define the list of known utility method names that should be excluded
    private static readonly HashSet<string> UtilityMethodNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "RemoveWarmupPeriods",
        "Condense",
        "Find",
        "Reset",
        "GetHashCode",
        "ToString",
        "Equals",
        "GetEnumerator"
    };

    // Define the list of utility-like method name prefixes
    private static readonly HashSet<string> UtilityPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Get",
        "Set",
        "Is",
        "Has",
        "Add",
        "Remove",
        "Update",
        "Calculate",
        "Convert",
        "Validate",
        "Format"
    };

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    /// <summary>
    /// Gets the diagnostics supported by this analyzer.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

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

        // Check if this is a public static method that might be an indicator instantiation method
        if (!IsPublicStaticMethod(methodDeclaration))
        {
            return;
        }

        // Get the method symbol
        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);
        if (methodSymbol == null)
        {
            return;
        }

        // Check if the method is marked as obsolete - skip it if it is
        if (HasObsoleteAttribute(methodSymbol))
        {
            return;
        }

        // Check if the method has the ExcludeFromCatalogAttribute - skip it if it does
        if (HasExcludeFromCatalogAttribute(methodSymbol))
        {
            return;
        }

        // Skip if this is a known utility method
        if (IsUtilityMethod(methodSymbol))
        {
            return;
        }

        // Check if it returns an indicator result type
        if (!IsIndicatorEntryPointMethod(methodDeclaration, methodSymbol, context.SemanticModel))
        {
            return;
        }

        // Now check if it has any attribute derived from IndicatorAttribute
        if (!HasIndicatorAttribute(methodDeclaration, context.SemanticModel))
        {
            Diagnostic diagnostic = Diagnostic.Create(
                Rule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }
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

    private static bool IsUtilityMethod(IMethodSymbol methodSymbol)
    {
        // Check against our list of known utility method names
        if (UtilityMethodNames.Contains(methodSymbol.Name))
        {
            return true;
        }

        // Check if method name starts with a utility prefix and doesn't start with "To"
        if (!methodSymbol.Name.StartsWith("To") &&
            UtilityPrefixes.Any(prefix => methodSymbol.Name.StartsWith(prefix)))
        {
            return true;
        }

        // Check if this method is in a utility class
        string containingClassName = methodSymbol.ContainingType.Name;
        return containingClassName.EndsWith("Utilities") ||
            containingClassName.EndsWith("Helper") ||
            containingClassName.EndsWith("Extensions") ||
            containingClassName.Contains("Util");
    }

    private static bool IsIndicatorEntryPointMethod(MethodDeclarationSyntax methodDeclaration, IMethodSymbol methodSymbol, SemanticModel semanticModel)
    {
        // Method name indicators - indicator entry points often start with "To"
        if (methodSymbol.Name.StartsWith("To") && methodSymbol.Name.Length > 2 && char.IsUpper(methodSymbol.Name[2]))
        {
            // Make sure it also returns an indicator result type
            return ReturnsIndicatorResultType(methodDeclaration, semanticModel);
        }

        // If the method name doesn't start with "To", it should still return an indicator result type
        // to be considered as an entry point
        return ReturnsIndicatorResultType(methodDeclaration, semanticModel);
    }

    private static bool ReturnsIndicatorResultType(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        // Get the method symbol to analyze its return type
        if (semanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol)
        {
            return false;
        }

        // Check if the return type name ends with "Result" or matches other indicator patterns
        ITypeSymbol returnType = methodSymbol.ReturnType;
        string returnTypeName = returnType.ToString();

        // Check for collections of results
        if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            // Check for IEnumerable<T>, IReadOnlyList<T>, etc. where T ends with "Result"
            ITypeSymbol typeArg = namedType.TypeArguments[0];
            if (typeArg.Name.EndsWith("Result"))
            {
                return true;
            }
        }

        // Check for Hub types (like EmaHub)
        return returnType.Name.EndsWith("Hub");
    }

    private static bool HasIndicatorAttribute(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        if (method.AttributeLists.Count == 0)
        {
            return false;
        }

        // Get the semantic model to resolve attribute types
        foreach (AttributeListSyntax attributeList in method.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                ISymbol? attributeSymbol = semanticModel.GetSymbolInfo(attribute).Symbol;

                if (attributeSymbol == null)
                {
                    continue;
                }

                INamedTypeSymbol? attributeType = attributeSymbol.ContainingType;

                if (attributeType == null)
                {
                    continue;
                }

                // Check direct attribute names
                if (attributeType.Name is "SeriesAttribute" or "StreamHubAttribute" or "BufferAttribute")
                {
                    return true;
                }

                // Check if this attribute derives from IndicatorAttribute
                INamedTypeSymbol? baseType = attributeType.BaseType;
                while (baseType != null)
                {
                    if (baseType.Name == "IndicatorAttribute")
                    {
                        return true;
                    }

                    baseType = baseType.BaseType;
                }
            }
        }

        return false;
    }
}
