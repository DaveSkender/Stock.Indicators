namespace Generators.Analyzer.Rules;

/// <summary>
/// Rule that checks if indicator method parameters have the appropriate ParamAttribute.
/// </summary>
internal static class ParamAttributeRules
{
    /// <summary>
    /// Analyzes a method's parameters to check for correct ParamAttribute usage.
    /// </summary>
    public static void Analyze(SyntaxNodeAnalysisContext context, IMethodSymbol methodSymbol)
    {
        // Skip validation entirely if there are no parameters
        if (methodSymbol.Parameters.Length == 0)
        {
            return;
        }

        // Get the ParamAttribute<T> base type
        INamedTypeSymbol? paramAttributeBaseSymbol = GetParamAttributeBaseSymbol(context.Compilation);
        if (paramAttributeBaseSymbol == null)
        {
            return;
        }

        // Determine starting index - skip the first parameter if:
        // 1. It's an instance method (implicit 'this')
        // 2. It's an extension method with a common quote collection parameter
        int startIdx = GetParameterStartIndex(methodSymbol);

        // Check remaining parameters for ParamAttribute or any of its derived types
        for (int i = startIdx; i < methodSymbol.Parameters.Length; i++)
        {
            IParameterSymbol parameter = methodSymbol.Parameters[i];
            CheckParameterAttribute(context, parameter, methodSymbol, paramAttributeBaseSymbol);
        }
    }

    /// <summary>
    /// Determines the starting index for parameter validation.
    /// </summary>
    private static int GetParameterStartIndex(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.IsStatic)
        {
            // Regular instance method - skip the implicit 'this'
            return 1;
        }
        else if (methodSymbol.IsExtensionMethod && methodSymbol.Parameters.Length > 0)
        {
            // Extension method - check if first param is a common collection parameter
            IParameterSymbol firstParam = methodSymbol.Parameters[0];

            // Check if it's a common parameter that should be excluded
            if (AnalyzerUtils.IsCommonParameter(firstParam))
            {
                return 1;
            }
        }

        // Start at the first parameter
        return 0;
    }

    /// <summary>
    /// Checks if a parameter has the correct ParamAttribute.
    /// </summary>
    private static void CheckParameterAttribute(
        SyntaxNodeAnalysisContext context,
        IParameterSymbol parameter,
        IMethodSymbol methodSymbol,
        INamedTypeSymbol paramAttributeBaseSymbol)
    {
        // Check if the parameter has any attribute derived from ParamAttribute<T>
        bool hasParamAttribute = parameter.GetAttributes()
            .Any(attr => IsParamAttributeOrDerived(attr.AttributeClass, paramAttributeBaseSymbol));

        if (!hasParamAttribute)
        {
            // Report diagnostic for missing ParamAttribute
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.MissingParamRule,
                    parameter.Locations.FirstOrDefault(),
                    parameter.Name,
                    $"{methodSymbol.ContainingType.Name}.{methodSymbol.Name}"));
        }
        else
        {
            // Validate type match for ParamAttribute<T>
            ValidateParamAttributeType(parameter, context, paramAttributeBaseSymbol);
        }
    }

    /// <summary>
    /// Validates that the ParamAttribute's generic type matches the parameter type.
    /// </summary>
    private static void ValidateParamAttributeType(
        IParameterSymbol parameter,
        SyntaxNodeAnalysisContext context,
        INamedTypeSymbol paramAttributeBaseSymbol)
    {
        foreach (AttributeData attribute in parameter.GetAttributes())
        {
            if (IsParamAttributeOrDerived(attribute.AttributeClass, paramAttributeBaseSymbol))
            {
                string attributeClassName = attribute.AttributeClass?.Name ?? string.Empty;

                // Determine the appropriate attribute type for this parameter
                string expectedAttributeType = GetExpectedAttributeType(parameter);

                // Check for mismatches between parameter type and attribute type
                if (parameter.Type.SpecialType == SpecialType.System_Boolean && !attributeClassName.Contains("ParamBool"))
                {
                    // Boolean parameter should use ParamBool attribute
                    ReportTypeMismatch(context, parameter, attribute, "Boolean");
                }
                else if (parameter.Type.TypeKind == TypeKind.Enum && !attributeClassName.Contains("ParamEnum"))
                {
                    // Enum parameter should use ParamEnum attribute
                    ReportTypeMismatch(context, parameter, attribute, parameter.Type.Name);
                }
                else if (attributeClassName.Contains("ParamNum"))
                {
                    // For ParamNum, check if generic type matches parameter type
                    ITypeSymbol? paramAttributeTypeArg = attribute.AttributeClass?.TypeArguments.FirstOrDefault();

                    if (paramAttributeTypeArg != null && !SymbolEqualityComparer.Default.Equals(paramAttributeTypeArg, parameter.Type))
                    {
                        // Report diagnostic for type mismatch
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                DiagnosticDescriptors.TypeMismatchRule,
                                parameter.Locations.FirstOrDefault(),
                                parameter.Name,
                                $"{parameter.ContainingSymbol.ContainingType.Name}.{parameter.ContainingSymbol.Name}",
                                paramAttributeTypeArg.Name,
                                parameter.Type.Name));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reports a type mismatch diagnostic with a suggestion for the correct attribute type.
    /// </summary>
    private static void ReportTypeMismatch(
        SyntaxNodeAnalysisContext context,
        IParameterSymbol parameter,
        AttributeData attribute,
        string expectedType)
    {
        ITypeSymbol? paramAttributeTypeArg = attribute.AttributeClass?.TypeArguments.FirstOrDefault();

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.TypeMismatchRule,
                parameter.Locations.FirstOrDefault(),
                parameter.Name,
                $"{parameter.ContainingSymbol.ContainingType.Name}.{parameter.ContainingSymbol.Name}",
                paramAttributeTypeArg?.Name ?? "incorrect",
                expectedType));
    }

    /// <summary>
    /// Determines the expected attribute type for a parameter based on its type.
    /// </summary>
    private static string GetExpectedAttributeType(IParameterSymbol parameter) =>
        parameter.Type.SpecialType == SpecialType.System_Boolean
            ? "ParamBool"
            : parameter.Type.TypeKind == TypeKind.Enum ? "ParamEnum" : "ParamNum";

    /// <summary>
    /// Gets the base ParamAttribute<T> symbol from the compilation.
    /// </summary>
    private static INamedTypeSymbol? GetParamAttributeBaseSymbol(Compilation compilation) =>
        // Look for the generic ParamAttribute<T> base class
        compilation.GetTypeByMetadataName("Skender.Stock.Indicators.ParamAttribute`1");

    /// <summary>
    /// Determines if an attribute class is a ParamAttribute or derived from ParamAttribute<T>.
    /// </summary>
    private static bool IsParamAttributeOrDerived(
        INamedTypeSymbol? attributeClass,
        INamedTypeSymbol? baseParamAttributeSymbol)
    {
        if (attributeClass == null || baseParamAttributeSymbol == null)
        {
            return false;
        }

        // Check if this is a generic instantiation of ParamAttribute<T> or its derived classes
        if (attributeClass.IsGenericType && attributeClass.ConstructedFrom != null)
        {
            INamedTypeSymbol originalDefinition = attributeClass.OriginalDefinition;

            // Check if this is derived from ParamAttribute<T> by walking up the inheritance chain
            INamedTypeSymbol? currentType = originalDefinition;
            while (currentType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(currentType, baseParamAttributeSymbol))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }
        }

        // Check non-generic derived types (like ParamBoolAttribute)
        return attributeClass.BaseType != null && IsParamAttributeOrDerived(attributeClass.BaseType, baseParamAttributeSymbol);
    }
}
