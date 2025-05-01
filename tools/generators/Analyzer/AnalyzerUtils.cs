namespace Generators.Analyzer;

/// <summary>
/// Common utility methods used by multiple rule analyzers.
/// </summary>
internal static class AnalyzerUtils
{
    /// <summary>
    /// Checks if the given type is IndicatorAttribute or derived from it.
    /// </summary>
    public static bool IsIndicatorAttributeOrDerived(INamedTypeSymbol? symbol)
    {
        if (symbol == null)
        {
            return false;
        }

        // Check if this is IndicatorAttribute
        if (symbol.Name == "IndicatorAttribute" ||
            symbol.ToString() == "Skender.Stock.Indicators.IndicatorAttribute")
        {
            return true;
        }

        // Check base type recursively
        return symbol.BaseType != null && IsIndicatorAttributeOrDerived(symbol.BaseType);
    }

    /// <summary>
    /// Checks if a method has the ObsoleteAttribute.
    /// </summary>
    public static bool HasObsoleteAttribute(IMethodSymbol methodSymbol)
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

    /// <summary>
    /// Checks if a method has the ExcludeFromCatalogAttribute.
    /// </summary>
    public static bool HasExcludeFromCatalogAttribute(IMethodSymbol methodSymbol)
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

    /// <summary>
    /// Determines if a parameter is a common parameter typically used in extensions.
    /// </summary>
    public static bool IsCommonParameter(IParameterSymbol parameter)
    {
        // Common parameter names for collections
        HashSet<string> commonParamNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "quotes",
            "source",
            "candles",
            "quoteProvider",
            "chainProvider",
            "provider"
        };

        // Check parameter name
        if (!commonParamNames.Contains(parameter.Name))
        {
            return false;
        }

        // Get the parameter type
        ITypeSymbol type = parameter.Type;

        // Check for hub provider interface types first
        string typeFullName = type.ToString();
        if (typeFullName.Contains("IQuoteProvider") ||
            typeFullName.Contains("IChainProvider"))
        {
            return true;
        }

        // For generic types, check if they are collections
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            string typeName = namedType.ToString();

            // Check if it's a collection type
            bool isCollectionType =
                typeName.Contains("IEnumerable") ||
                typeName.Contains("IReadOnlyList") ||
                typeName.Contains("List") ||
                typeName.Contains("IList") ||
                typeName.Contains("ICollection");

            // If it's a collection with at least one type parameter, check it
            if (isCollectionType && namedType.TypeArguments.Length > 0)
            {
                // For "source" parameter, check the type constraints if available
                if (string.Equals(parameter.Name, "source", StringComparison.OrdinalIgnoreCase))
                {
                    // Find the containing method to check type parameter constraints
                    if (parameter.ContainingSymbol is IMethodSymbol methodSymbol &&
                        methodSymbol.IsGenericMethod)
                    {
                        foreach (ITypeParameterSymbol typeParam in methodSymbol.TypeParameters)
                        {
                            // Look for type parameters that have constraints
                            if (typeParam.ConstraintTypes.Length > 0)
                            {
                                // Check if any constraint type is IReusable or similar common interfaces
                                foreach (ITypeSymbol constraintType in typeParam.ConstraintTypes)
                                {
                                    if (constraintType.Name.Contains("Reusable") ||
                                        constraintType.Name.Contains("Quote") ||
                                        constraintType.Name.Contains("Series") ||
                                        constraintType.Name.Contains("Bar"))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    // Even if we can't find specific constraints, still be lenient with "source"
                    return true;
                }

                // For other parameter names, check if element type is quote-like
                ITypeSymbol elementType = namedType.TypeArguments[0];
                string elementTypeName = elementType.Name;

                // Check if element type is quote-like
                bool isQuoteType =
                    elementTypeName.Contains("Quote") ||
                    elementTypeName.Contains("Candle") ||
                    elementTypeName.EndsWith("Bar") ||
                    elementTypeName == "TQuote" ||
                    elementTypeName.Contains("Reusable");

                return isQuoteType;
            }
        }

        return false;
    }
}