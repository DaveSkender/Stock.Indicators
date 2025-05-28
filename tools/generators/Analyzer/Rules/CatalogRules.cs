namespace Generators.Analyzer.Rules;

/// <summary>
/// Rules for validating indicator listings in the catalog system.
/// </summary>
internal static class CatalogRules
{
    /// <summary>
    /// Analyzes a class to check if it has indicator attributes but no Listing property.
    /// </summary>
    public static void AnalyzeMissingListing(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        // Get the class symbol
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        // Skip abstract classes and interfaces
        if (classSymbol.IsAbstract || classSymbol.TypeKind == TypeKind.Interface)
        {
            return;
        }

        // Check if the class has any indicator attributes
        bool hasIndicatorAttribute = false;
        foreach (AttributeData attribute in classSymbol.GetAttributes())
        {
            if (IsIndicatorRelatedAttribute(attribute.AttributeClass))
            {
                hasIndicatorAttribute = true;
                break;
            }
        }

        if (!hasIndicatorAttribute)
        {
            return;
        }

        // Check if the class has a static Listing property of type IndicatorListing
        bool hasListingProperty = false;
        foreach (ISymbol member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property &&
                property.Name == "Listing" &&
                property.IsStatic &&
                IsIndicatorListingType(property.Type))
            {
                hasListingProperty = true;
                break;
            }
        }

        // Report diagnostic if the class has indicator attributes but no Listing property
        if (!hasListingProperty)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                CatalogDiagnosticDescriptors.SID001_MissingListingDescriptor,
                classDeclaration.Identifier.GetLocation(),
                classDeclaration.Identifier.Text));
        }
    }

    /// <summary>
    /// Analyzes parameter consistency between implementations and catalog listings.
    /// </summary>
    public static void AnalyzeParametersConsistency(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        // Get the class symbol
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        // Find the Listing property
        IPropertySymbol? listingProperty = null;
        foreach (ISymbol member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property &&
                property.Name == "Listing" &&
                property.IsStatic &&
                IsIndicatorListingType(property.Type))
            {
                listingProperty = property;
                break;
            }
        }

        if (listingProperty == null)
        {
            return; // No Listing property to compare with
        }

        // Get all public methods and constructors in the class
        List<IMethodSymbol> methodsToAnalyze = [];

        // Add constructors
        foreach (IMethodSymbol constructor in classSymbol.Constructors)
        {
            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                methodsToAnalyze.Add(constructor);
            }
        }

        // Add methods
        foreach (ISymbol member in classSymbol.GetMembers())
        {
            if (member is IMethodSymbol method &&
                method.MethodKind == MethodKind.Ordinary &&
                method.DeclaredAccessibility == Accessibility.Public &&
                !method.IsOverride &&
                !method.IsStatic &&
                !AnalyzerUtils.HasExcludeFromCatalogAttribute(method))
            {
                methodsToAnalyze.Add(method);
            }
        }

        // Get parameters from listing property (if we can find the initialization)
        Dictionary<string, (Location Location, string Type)> listingParameters = GetParametersFromListingProperty(context, listingProperty);

        // Collect all parameters from implementation methods
        Dictionary<string, (Location Location, string Type, IMethodSymbol Method)> implementationParameters = new(StringComparer.OrdinalIgnoreCase);

        foreach (IMethodSymbol method in methodsToAnalyze)
        {
            foreach (IParameterSymbol parameter in method.Parameters)
            {
                // Skip common parameters that aren't specific to the indicator configuration
                if (AnalyzerUtils.IsCommonParameter(parameter))
                {
                    continue;
                }

                // Add to implementation parameters if not already present
                string paramName = parameter.Name.ToLowerInvariant();
                string paramType = parameter.Type.ToString();

                if (!implementationParameters.ContainsKey(paramName))
                {
                    implementationParameters.Add(
                        paramName,
                        (parameter.Locations[0], paramType, method));
                }
            }
        }

        // Check for missing parameters (parameters in implementation but not in listing)
        foreach (KeyValuePair<string, (Location Location, string Type, IMethodSymbol Method)> kvp in implementationParameters)
        {
            string paramName = kvp.Key;
            Location location = kvp.Value.Location;
            string paramType = kvp.Value.Type;
            IMethodSymbol method = kvp.Value.Method;

            if (!listingParameters.TryGetValue(paramName, out (Location Location, string Type) value))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    CatalogDiagnosticDescriptors.SID002_MissingParameterDescriptor,
                    location,
                    paramName,
                    method.Name));
            }
            else
            {
                // Check for type mismatches
                string listingType = value.Type;
                if (!AreParameterTypesCompatible(paramType, listingType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        CatalogDiagnosticDescriptors.SID004_ParameterTypeMismatchDescriptor,
                        location,
                        paramName,
                        paramType,
                        listingType));
                }
            }
        }

        // Check for extraneous parameters (parameters in listing but not in implementation)
        foreach (KeyValuePair<string, (Location Location, string Type)> kvp in listingParameters)
        {
            string paramName = kvp.Key;
            Location location = kvp.Value.Location;

            if (!implementationParameters.ContainsKey(paramName))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    CatalogDiagnosticDescriptors.SID003_ExtraneousParameterDescriptor,
                    location,
                    paramName));
            }
        }
    }

    /// <summary>
    /// Analyzes result property consistency between implementations and catalog listings.
    /// </summary>
    public static void AnalyzeResultsConsistency(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
    {
        // Get the class symbol
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            return;
        }

        // Find the Listing property
        IPropertySymbol? listingProperty = null;
        foreach (ISymbol member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property &&
                property.Name == "Listing" &&
                property.IsStatic &&
                IsIndicatorListingType(property.Type))
            {
                listingProperty = property;
                break;
            }
        }

        if (listingProperty == null)
        {
            return; // No Listing property to compare with
        }

        // Get all result properties or methods in indicator classes
        List<(string Name, Location Location)> resultProperties = GetResultPropertiesFromReturnTypes(context, classSymbol);

        // Get results from listing property
        HashSet<string> listingResults = GetResultsFromListingProperty(context, listingProperty);

        // Check for missing results (properties in implementation but not in listing)
        foreach ((string Name, Location Location) resultProperty in resultProperties)
        {
            string name = resultProperty.Name;
            Location location = resultProperty.Location;

            if (!listingResults.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    CatalogDiagnosticDescriptors.SID005_MissingResultDescriptor,
                    location,
                    name));
            }
        }
    }

    /// <summary>
    /// Determines if a type is indicator-related attribute.
    /// </summary>
    private static bool IsIndicatorRelatedAttribute(INamedTypeSymbol? type)
    {
        if (type == null)
        {
            return false;
        }

        // Check for any of our indicator attribute types
        string typeName = type.ToString();
        return typeName.Contains("SeriesIndicatorAttribute") ||
               typeName.Contains("StreamIndicatorAttribute") ||
               typeName.Contains("BufferIndicatorAttribute") ||
               typeName.Contains("IndicatorAttribute");
    }

    /// <summary>
    /// Determines if a type is IndicatorListing.
    /// </summary>
    private static bool IsIndicatorListingType(ITypeSymbol type)
    {
        string typeName = type.ToString();
        return typeName == "Skender.Stock.Indicators.IndicatorListing" ||
               typeName.EndsWith(".IndicatorListing") ||
               typeName == "Skender.Stock.Indicators.CompositeIndicatorListing" ||
               typeName.EndsWith(".CompositeIndicatorListing");
    }

    /// <summary>
    /// Gets parameters from a static Listing property initialization.
    /// </summary>
    private static Dictionary<string, (Location Location, string Type)> GetParametersFromListingProperty(
        SyntaxNodeAnalysisContext context, IPropertySymbol listingProperty)
    {
        Dictionary<string, (Location Location, string Type)> parameters = new(StringComparer.OrdinalIgnoreCase);

        // Find property declaration and initialization
        foreach (SyntaxReference declaringSyntax in listingProperty.DeclaringSyntaxReferences)
        {
            if (declaringSyntax.GetSyntax() is PropertyDeclarationSyntax propDeclaration)
            {
                // Look for initialization expressions like:
                // .AddParameter<int>("lookbackPeriods", ...)
                // or any method that adds parameters to the builder

                // First find the property initializer
                if (propDeclaration.Initializer?.Value is ObjectCreationExpressionSyntax objectCreation)
                {
                    ExtractParametersFromObjectCreation(parameters, objectCreation, context);
                }
                else if (propDeclaration.Initializer?.Value is InvocationExpressionSyntax invocation)
                {
                    ExtractParametersFromMethodChain(parameters, invocation, context);
                }
            }
        }

        return parameters;
    }

    /// <summary>
    /// Extracts parameters from an object creation expression.
    /// </summary>
    private static void ExtractParametersFromObjectCreation(
        Dictionary<string, (Location, string)> parameters,
        ObjectCreationExpressionSyntax objectCreation,
        SyntaxNodeAnalysisContext context)
    {
        // Handle direct object creation with parameter initializers
        // TODO: Implement if needed based on codebase patterns
    }

    /// <summary>
    /// Extracts parameters from a method invocation chain.
    /// </summary>
    private static void ExtractParametersFromMethodChain(
        Dictionary<string, (Location, string)> parameters,
        InvocationExpressionSyntax invocation,
        SyntaxNodeAnalysisContext context)
    {
        // Follow the method chain to find all AddParameter calls
        SyntaxNode? current = invocation;
        while (current != null)
        {
            if (current is InvocationExpressionSyntax currentInvocation)
            {
                // Check if this is an AddParameter call
                if (IsAddParameterMethod(currentInvocation, out string? paramName, out string? paramType))
                {
                    if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramType))
                    {
                        parameters[paramName!] = (currentInvocation.GetLocation(), paramType!);
                    }
                }

                // Move to next in chain (could be on the LHS of this invocation)
                current = currentInvocation.Expression is MemberAccessExpressionSyntax memberAccess
                    ? memberAccess.Expression
                    : (SyntaxNode?)null;
            }
            else
            {
                current = null;
            }
        }
    }

    /// <summary>
    /// Determines if an invocation is an AddParameter method and extracts parameter name and type.
    /// </summary>
    private static bool IsAddParameterMethod(
        InvocationExpressionSyntax invocation,
        out string? paramName,
        out string? paramType)
    {
        paramName = null;
        paramType = null;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text.StartsWith("AddParameter"))
        {
            // Get the parameter name from the first argument
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                if (invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literal &&
                    literal.Token.ValueText is string name)
                {
                    paramName = name;
                }
            }

            // Try to get the parameter type from generic arguments
            if (memberAccess.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Count > 0)
            {
                paramType = genericName.TypeArgumentList.Arguments[0].ToString();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets result properties from return types of indicator methods.
    /// </summary>
    private static List<(string Name, Location Location)> GetResultPropertiesFromReturnTypes(
        SyntaxNodeAnalysisContext context,
        INamedTypeSymbol classSymbol)
    {
        List<(string Name, Location Location)> resultProperties = [];

        // Get all public methods in the class
        foreach (ISymbol member in classSymbol.GetMembers())
        {
            if (member is IMethodSymbol method &&
                method.MethodKind == MethodKind.Ordinary &&
                method.DeclaredAccessibility == Accessibility.Public &&
                !method.IsStatic)
            {
                // Skip methods with common return types that don't define indicator results
                if (IsCommonMethodToSkip(method))
                {
                    continue;
                }

                // Analyze the return type for properties
                ExtractResultPropertiesFromReturnType(method.ReturnType, resultProperties, method.Locations[0], context.Compilation);
            }
        }

        return resultProperties;
    }

    /// <summary>
    /// Determines if a method should be skipped for result analysis.
    /// </summary>
    private static bool IsCommonMethodToSkip(IMethodSymbol method)
    {
        // Skip common methods that don't return indicator results
        string methodName = method.Name.ToLowerInvariant();
        return methodName == "tostring" ||
               methodName == "equals" ||
               methodName == "gethashcode" ||
               methodName.StartsWith("get_") ||
               methodName.StartsWith("set_");
    }

    /// <summary>
    /// Extracts result properties from a return type.
    /// </summary>
    private static void ExtractResultPropertiesFromReturnType(
        ITypeSymbol returnType,
        List<(string Name, Location Location)> resultProperties,
        Location location,
        Compilation compilation)
    {
        // Handle collection types
        if (returnType is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            string typeName = namedType.ToString();

            // Check if it's a collection type
            bool isCollection = typeName.Contains("IEnumerable") ||
                                typeName.Contains("IReadOnlyList") ||
                                typeName.Contains("List");

            if (isCollection && namedType.TypeArguments.Length > 0)
            {
                // Get the element type and extract its properties
                ITypeSymbol elementType = namedType.TypeArguments[0];
                ExtractPropertiesFromType(elementType, resultProperties, location);
            }
        }
        else
        {
            // Direct return type, extract its properties
            ExtractPropertiesFromType(returnType, resultProperties, location);
        }
    }

    /// <summary>
    /// Extracts properties from a type that could be indicator results.
    /// </summary>
    private static void ExtractPropertiesFromType(
        ITypeSymbol type,
        List<(string Name, Location Location)> resultProperties,
        Location location)
    {
        // Skip primitive and common non-indicator types
        if (IsPrimitiveOrCommonType(type))
        {
            return;
        }

        // Get all public properties
        foreach (ISymbol member in type.GetMembers())
        {
            if (member is IPropertySymbol property &&
                property.DeclaredAccessibility == Accessibility.Public &&
                !property.IsStatic)
            {
                // Skip common properties not related to indicator results
                if (!IsCommonPropertyToSkip(property))
                {
                    resultProperties.Add((property.Name, location));
                }
            }
        }
    }

    /// <summary>
    /// Determines if a type is primitive or a common non-indicator type.
    /// </summary>
    private static bool IsPrimitiveOrCommonType(ITypeSymbol type)
    {
        string typeName = type.ToString();

        // Check for primitive types
        if (type.IsValueType || type.SpecialType != SpecialType.None)
        {
            return true;
        }

        // Check for common non-indicator types
        return typeName is "System.String" or
               "System.Object" or
               "System.DateTime";
    }

    /// <summary>
    /// Determines if a property should be skipped for result analysis.
    /// </summary>
    private static bool IsCommonPropertyToSkip(IPropertySymbol property)
    {
        // Skip common properties that are likely not indicator results
        string propName = property.Name.ToLowerInvariant();
        return propName is "date" or
               "datetime" or
               "timestamp" or
               "period";
    }

    /// <summary>
    /// Gets results from a static Listing property initialization.
    /// </summary>
    private static HashSet<string> GetResultsFromListingProperty(
        SyntaxNodeAnalysisContext context,
        IPropertySymbol listingProperty)
    {
        HashSet<string> results = new(StringComparer.OrdinalIgnoreCase);

        // Find property declaration and initialization
        foreach (SyntaxReference declaringSyntax in listingProperty.DeclaringSyntaxReferences)
        {
            if (declaringSyntax.GetSyntax() is PropertyDeclarationSyntax propDeclaration)
            {
                // Look for initialization expressions like:
                // .AddResult("Ema", "EMA", ResultType.Default, isDefault: true)

                // First find the property initializer
                if (propDeclaration.Initializer?.Value is ObjectCreationExpressionSyntax objectCreation)
                {
                    ExtractResultsFromObjectCreation(results, objectCreation, context);
                }
                else if (propDeclaration.Initializer?.Value is InvocationExpressionSyntax invocation)
                {
                    ExtractResultsFromMethodChain(results, invocation, context);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Extracts results from an object creation expression.
    /// </summary>
    private static void ExtractResultsFromObjectCreation(
        HashSet<string> results,
        ObjectCreationExpressionSyntax objectCreation,
        SyntaxNodeAnalysisContext context)
    {
        // Handle direct object creation with result initializers
        // TODO: Implement if needed based on codebase patterns
    }

    /// <summary>
    /// Extracts results from a method invocation chain.
    /// </summary>
    private static void ExtractResultsFromMethodChain(
        HashSet<string> results,
        InvocationExpressionSyntax invocation,
        SyntaxNodeAnalysisContext context)
    {
        // Follow the method chain to find all AddResult calls
        SyntaxNode? current = invocation;
        while (current != null)
        {
            if (current is InvocationExpressionSyntax currentInvocation)
            {
                // Check if this is an AddResult call
                if (IsAddResultMethod(currentInvocation, out string? resultName))
                {
                    if (!string.IsNullOrEmpty(resultName))
                    {
                        results.Add(resultName!);
                    }
                }

                // Move to next in chain (could be on the LHS of this invocation)
                current = currentInvocation.Expression is MemberAccessExpressionSyntax memberAccess ? memberAccess.Expression : (SyntaxNode?)null;
            }
            else
            {
                current = null;
            }
        }
    }

    /// <summary>
    /// Determines if an invocation is an AddResult method and extracts result name.
    /// </summary>
    private static bool IsAddResultMethod(
        InvocationExpressionSyntax invocation,
        out string? resultName)
    {
        resultName = null;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            (memberAccess.Name.Identifier.Text == "AddResult" ||
             (memberAccess.Name.Identifier.Text.StartsWith("Add") &&
             memberAccess.Name.Identifier.Text.Contains("Result"))))
        {
            // Get the result name from the first argument
            if (invocation.ArgumentList.Arguments.Count > 0)
            {
                if (invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literal &&
                    literal.Token.ValueText is string name)
                {
                    resultName = name;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if two parameter types are compatible for the listing vs implementation.
    /// </summary>
    private static bool AreParameterTypesCompatible(string implementationType, string listingType)
    {
        // Normalize the types for comparison
        string normalizedImpl = NormalizeTypeName(implementationType);
        string normalizedListing = NormalizeTypeName(listingType);

        if (normalizedImpl == normalizedListing)
        {
            return true;
        }

        // Handle nullable types
        if (normalizedImpl.Contains("?") || normalizedListing.Contains("?"))
        {
            string implWithoutNullable = normalizedImpl.Replace("?", "").Trim();
            string listingWithoutNullable = normalizedListing.Replace("?", "").Trim();

            if (implWithoutNullable == listingWithoutNullable)
            {
                return true;
            }
        }

        // Handle Nullable<T> vs T? notation
        if (normalizedImpl.StartsWith("System.Nullable<") || normalizedListing.StartsWith("System.Nullable<"))
        {
            string implBaseType = ExtractNullableBaseType(normalizedImpl);
            string listingBaseType = ExtractNullableBaseType(normalizedListing);

            if (implBaseType == listingBaseType)
            {
                return true;
            }
        }

        // Handle common type alias differences
        if (IsTypeAliasMatch(normalizedImpl, normalizedListing))
        {
            return true;
        }

        // TODO: Add more specific compatibility checks if needed

        return false;
    }

    /// <summary>
    /// Normalizes a type name for comparison.
    /// </summary>
    private static string NormalizeTypeName(string typeName) => typeName.Trim()
            .Replace("System.Collections.Generic.", "")
            .Replace("System.", "");

    /// <summary>
    /// Extracts the base type from a Nullable<T> type.
    /// </summary>
    private static string ExtractNullableBaseType(string typeName)
    {
        if (typeName.StartsWith("System.Nullable<") || typeName.StartsWith("Nullable<"))
        {
            int start = typeName.IndexOf('<') + 1;
            int end = typeName.LastIndexOf('>');
            if (start > 0 && end > start)
            {
                return typeName.Substring(start, end - start);
            }
        }

        return typeName;
    }

    /// <summary>
    /// Checks if two types match considering C# type aliases.
    /// </summary>
    private static bool IsTypeAliasMatch(string type1, string type2)
    {
        Dictionary<string, HashSet<string>> aliasMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Int32", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "int" } },
            { "Int64", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "long" } },
            { "String", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "string" } },
            { "Boolean", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "bool" } },
            { "Double", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "double" } },
            { "Decimal", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "decimal" } },
            { "Single", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "float" } }
        };

        foreach (KeyValuePair<string, HashSet<string>> pair in aliasMap)
        {
            string baseType = pair.Key;
            HashSet<string> aliases = pair.Value;

            if ((type1.Equals(baseType, StringComparison.OrdinalIgnoreCase) || aliases.Contains(type1)) &&
                (type2.Equals(baseType, StringComparison.OrdinalIgnoreCase) || aliases.Contains(type2)))
            {
                return true;
            }
        }

        return false;
    }
}
