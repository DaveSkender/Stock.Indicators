namespace Generators.Analyzer.Rules;

/// <summary>
/// Rule that checks if indicator methods have the appropriate style-specific catalog attribute.
/// </summary>
internal static class IndicatorStyleRules
{
    // Define constant for "not an indicator"
    private const int StyleNone = -1;

    /// <summary>
    /// Analyzes a method to see if it has the appropriate style attribute.
    /// </summary>
    public static void Analyze(SyntaxNodeAnalysisContext context, IMethodSymbol methodSymbol, MethodDeclarationSyntax methodDeclaration)
    {
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
                DiagnosticDescriptors.SeriesRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text));
        }
        else if (styleValue == 2 && !HasAttributeOfType(methodDeclaration, "StreamAttribute", context.SemanticModel))
        {
            // Stream style indicator missing StreamAttribute
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.StreamRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text));
        }
        else if (styleValue == 1 && !HasAttributeOfType(methodDeclaration, "BufferAttribute", context.SemanticModel))
        {
            // Buffer style indicator missing BufferAttribute
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.BufferRule,
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
    public static int DetermineIndicatorStyle(IMethodSymbol methodSymbol, Compilation compilation)
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

    /// <summary>
    /// Determines if a type is a Stream type.
    /// </summary>
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

    /// <summary>
    /// Determines if a type is a Buffer type.
    /// </summary>
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

    /// <summary>
    /// Determines if a type is a Series type.
    /// </summary>
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

    /// <summary>
    /// Checks if a method has an attribute of a specific type.
    /// </summary>
    public static bool HasAttributeOfType(MethodDeclarationSyntax method, string attributeName, SemanticModel semanticModel)
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

            // Check if derived from IndicatorAttribute and look at Style property
            if (AnalyzerUtils.IsIndicatorAttributeOrDerived(attributeClass))
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
}
