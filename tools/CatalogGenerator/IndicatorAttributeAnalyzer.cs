using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Stock.Indicators.Generator;

/// <summary>
/// Analyzer that checks if indicator public instantiation methods have the required IndicatorAttribute-derived attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IndicatorAttributeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "SKI0001";
    private const string Title = "Indicator method missing required attribute";
    private const string MessageFormat = "Public indicator method '{0}' must have one of the derived attribute types of IndicatorAttribute";
    private const string Description = "All indicator public instantiation methods should have one of the derived attribute types of IndicatorAttribute abstract class.";
    private const string Category = "Usage";

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

        // Check if it returns an indicator result type
        if (!ReturnsIndicatorResultType(methodDeclaration, context.SemanticModel))
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

    private static bool IsPublicStaticMethod(MethodDeclarationSyntax method) => method.Modifiers.Any(SyntaxKind.PublicKeyword) &&
               method.Modifiers.Any(SyntaxKind.StaticKeyword);

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
