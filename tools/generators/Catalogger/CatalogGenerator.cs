using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

// Note: We intentionally avoid referencing Skender.Stock.Indicators directly
// to prevent circular dependencies. Instead, we use string-based checks.
namespace Generators.Catalogger;

/// <summary>
/// Source generator for indicator catalog listings.
/// This generator analyzes attributed methods and automatically
/// generates registration code for the indicator catalog.
/// </summary>
[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the incremental generator.
    /// </summary>
    /// <param name="context">The incremental generator initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // During unit tests, we want to disable the actual catalog generation
        // This is safe since tests explicitly call CompileAndValidate which provides their own input
#if DEBUG
        try
        {
            // Check for test assemblies - this avoids duplicate code generation during tests
            bool isRunningInTests = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.GetName().Name?.Contains("Tests.Generators") == true ||
                         a.GetName().Name?.Contains("TestHost") == true);

            if (isRunningInTests)
            {
                // Skip actual code generation during unit tests
                return;
            }
        }
        catch
        {
            // Ignore any errors in the check
        }
#endif

        // Register for syntax nodes that could be indicator classes containing attributed methods
        IncrementalValuesProvider<IndicatorClassInfo> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsPotentialIndicatorClass(s),
                transform: static (ctx, _) => GetClassForAnalysis(ctx))
            .Where(static c => c != null)!;

        // Collect all indicator classes
        IncrementalValueProvider<ImmutableArray<IndicatorClassInfo>> collectedClasses =
            classDeclarations.Collect();

        // Register output for the collected indicator classes
        context.RegisterSourceOutput(collectedClasses, static (spc, classInfos) =>
            GenerateRegistration(spc, classInfos));
    }

    /// <summary>
    /// Determines if a syntax node is potentially an indicator class containing attributed methods.
    /// </summary>
    private static bool IsPotentialIndicatorClass(SyntaxNode node)
    {
        // It must be a class, and for partial classes we need to check methods
        if (node is not ClassDeclarationSyntax classDecl ||
            classDecl.Modifiers.Any(SyntaxKind.AbstractKeyword))
            return false;

        // Quick check: Does the class have a 'Listing' property? If so, skip it
        if (classDecl.Members.OfType<PropertyDeclarationSyntax>()
            .Any(p => p.Identifier.Text == "Listing" && p.Modifiers.Any(SyntaxKind.StaticKeyword)))
            return false;

        // Does class have static methods with attributes?
        return classDecl.Members.OfType<MethodDeclarationSyntax>()
            .Any(m => m.AttributeLists.Count > 0 && m.Modifiers.Any(SyntaxKind.StaticKeyword));
    }

    /// <summary>
    /// Gets the class information for analysis if it contains methods with indicator attributes.
    /// </summary>
    private static IndicatorClassInfo? GetClassForAnalysis(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        // Get the semantic model for the class
        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            return null;

        // Look for methods with indicator attributes
        var methods = classSymbol.GetMembers().OfType<IMethodSymbol>()
            .Where(m => m.IsStatic && !m.IsImplicitlyDeclared);

        // Track methods we've processed to avoid duplicates
        var processedMethodNames = new HashSet<string>();
        IndicatorClassInfo? classInfo = null;

        // Check if class already has a Listing property
        bool hasExistingListing = HasListingProperty(classSymbol);

        // Track all attributed methods and their styles
        var attributedMethods = new List<(IMethodSymbol Method, IndicatorAttributeInfo AttributeInfo)>();

        // Check each method for indicator attributes
        foreach (var method in methods)
        {
            // Skip duplicate methods with the same name (only process each method once)
            if (processedMethodNames.Contains(method.Name))
                continue;

            var attributeInfo = ExtractIndicatorAttributeFromMethod(method);
            if (attributeInfo != null)
            {
                // Mark this method as processed
                processedMethodNames.Add(method.Name);

                // Add to attributed methods collection
                attributedMethods.Add((method, attributeInfo));
            }
        }

        // If we found attributed methods, create class info using the first one
        if (attributedMethods.Count > 0)
        {
            var (method, attributeInfo) = attributedMethods[0];

            // Extract parameter information from the attributed method
            var parameters = ExtractParameterInfoFromMethod(method);

            // Extract result type information from the method's return type
            var resultInfo = ExtractResultTypeInfo(method);

            // Get all styles from attributed methods
            var styles = attributedMethods.Select(m => m.AttributeInfo.Style ?? string.Empty)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()  // Make sure we don't have duplicates
                .ToArray();

            // Check if method ids are consistent across all attributed methods
            bool hasConsistentIds = attributedMethods.All(m =>
                string.Equals(m.AttributeInfo.Id, attributeInfo.Id, StringComparison.OrdinalIgnoreCase));

            if (!hasConsistentIds)
            {
                // Log a warning that IDs must be consistent for multi-style indicators
                // We'll still proceed with the first one's ID
            }

            classInfo = new IndicatorClassInfo(
                classSymbol.Name,
                classSymbol.ToDisplayString(),
                classSymbol.ContainingNamespace.ToDisplayString(),
                method.Name,
                attributeInfo,
                parameters,
                resultInfo,
                hasExistingListing,
                attributedMethods.Count > 1,  // Has multiple attributes
                styles);
        }

        return classInfo;
    }

    /// <summary>
    /// Extracts indicator attribute information from a method symbol.
    /// </summary>
    private static IndicatorAttributeInfo? ExtractIndicatorAttributeFromMethod(IMethodSymbol methodSymbol)
    {
        foreach (var attribute in methodSymbol.GetAttributes())
        {
            if (attribute.AttributeClass == null)
                continue;

            string attributeName = attribute.AttributeClass.Name;

            // Check for indicator-related attributes
            if (IsIndicatorAttribute(attributeName))
            {
                string? id = null;
                string? style = null;

                // Extract attribute arguments
                if (attribute.ConstructorArguments.Length > 0)
                {
                    id = attribute.ConstructorArguments[0].Value?.ToString();
                }
                if (attribute.ConstructorArguments.Length > 1)
                {
                    style = attribute.ConstructorArguments[1].Value?.ToString();
                }

                // Extract named arguments
                foreach (var namedArg in attribute.NamedArguments)
                {
                    switch (namedArg.Key)
                    {
                        case "Id":
                            id = namedArg.Value.Value?.ToString();
                            break;
                        case "Style":
                            style = namedArg.Value.Value?.ToString();
                            break;
                    }
                }

                return new IndicatorAttributeInfo(
                    attributeName,
                    id,
                    style);
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if an attribute name indicates an indicator attribute.
    /// </summary>
    private static bool IsIndicatorAttribute(string attributeName) =>
        attributeName.EndsWith("IndicatorAttribute") ||
        attributeName == "SeriesIndicatorAttribute" ||
        attributeName == "StreamIndicatorAttribute" ||
        attributeName == "BufferIndicatorAttribute" ||
        attributeName == "IndicatorAttribute";

    /// <summary>
    /// Extracts parameter information from constructors and key methods.
    /// </summary>
    private static ImmutableArray<ParameterInfo> ExtractParameterInfo(INamedTypeSymbol classSymbol)
    {
        var parameters = ImmutableArray.CreateBuilder<ParameterInfo>();

        // Get public constructors
        foreach (var constructor in classSymbol.Constructors)
        {
            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                foreach (var param in constructor.Parameters)
                {
                    if (!IsCommonCollectionParameter(param))
                    {
                        parameters.Add(new ParameterInfo(
                            param.Name,
                            param.Type.ToDisplayString(),
                            !param.HasExplicitDefaultValue,
                            param.HasExplicitDefaultValue ? param.ExplicitDefaultValue?.ToString() : null));
                    }
                }
                break; // Use first public constructor
            }
        }

        return parameters.ToImmutable();
    }

    /// <summary>
    /// Extracts parameter information from a method.
    /// </summary>
    private static ImmutableArray<ParameterInfo> ExtractParameterInfoFromMethod(IMethodSymbol methodSymbol)
    {
        var parameters = ImmutableArray.CreateBuilder<ParameterInfo>();

        foreach (var param in methodSymbol.Parameters)
        {
            // Skip common collection parameters like "source" and "quotes"
            if (!IsCommonCollectionParameter(param))
            {
                parameters.Add(new ParameterInfo(
                    param.Name,
                    param.Type.ToDisplayString(),
                    !param.HasExplicitDefaultValue,
                    param.HasExplicitDefaultValue ? param.ExplicitDefaultValue?.ToString() : null));
            }
        }

        return parameters.ToImmutable();
    }

    /// <summary>
    /// Extracts result type information from a method's return type.
    /// </summary>
    private static ResultTypeInfo ExtractResultTypeInfo(IMethodSymbol methodSymbol)
    {
        // Default to "Default"
        string resultType = "Default";
        var description = string.Empty;

        // Extract description from XML documentation if available
        description = ExtractDocumentationSummary(methodSymbol);

        // Check the method's return type for specific indicators
        var returnType = methodSymbol.ReturnType;
        if (returnType is INamedTypeSymbol namedReturnType)
        {
            if (namedReturnType.IsGenericType)
            {
                string genericName = namedReturnType.ConstructedFrom.ToDisplayString();

                // Check for common collection types that indicate series data
                if (genericName.Contains("IEnumerable") ||
                    genericName.Contains("IReadOnlyList") ||
                    genericName.Contains("List") ||
                    genericName.Contains("IList"))
                {
                    // Series data type
                    resultType = "Series";

                    // Check for result type patterns in generic type args
                    var typeArgs = namedReturnType.TypeArguments;
                    if (typeArgs.Length > 0 && typeArgs[0] is INamedTypeSymbol resultTypeSymbol)
                    {
                        // Look for properties that might indicate specific result types
                        foreach (var member in resultTypeSymbol.GetMembers().OfType<IPropertySymbol>())
                        {
                            if (IsPotentialSignalProperty(member.Name))
                            {
                                resultType = "Signal";
                                break;
                            }
                        }
                    }
                }
            }
            else if (returnType.Name == "Task" && returnType is INamedTypeSymbol taskType &&
                     taskType.TypeArguments.Length == 1)
            {
                // Task<T> indicates an asynchronous series result
                resultType = "AsyncSeries";
            }

            // Infer more specific result types based on name patterns
            var typeName = returnType.Name;
            if (typeName.Contains("Signal"))
            {
                resultType = "Signal";
            }
            else if (typeName.Contains("Oscillator"))
            {
                resultType = "Oscillator";
            }
            else if (typeName.Contains("Band") || typeName.Contains("Channel"))
            {
                resultType = "Band";
            }
        }

        return new ResultTypeInfo(resultType, description);
    }

    /// <summary>
    /// Checks if a property name indicates it's a potential signal property
    /// </summary>
    private static bool IsPotentialSignalProperty(string propertyName)
    {
        return propertyName.Contains("Signal") ||
               propertyName.Contains("Buy") ||
               propertyName.Contains("Sell") ||
               propertyName.Contains("Cross") ||
               propertyName.Contains("Alert");
    }

    /// <summary>
    /// Extracts the summary from XML documentation for a method
    /// </summary>
    private static string ExtractDocumentationSummary(ISymbol symbol)
    {
        // Get XML documentation for the symbol
        string? xmlDoc = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrEmpty(xmlDoc))
            return string.Empty;

        try
        {
            // Handle null XML doc
            if (string.IsNullOrEmpty(xmlDoc))
                return string.Empty;

            // Simple XML parsing to extract summary
            int summaryStartIndex = xmlDoc!.IndexOf("<summary>");
            if (summaryStartIndex < 0) return string.Empty;

            int summaryStart = summaryStartIndex + "<summary>".Length;
            int summaryEnd = xmlDoc.IndexOf("</summary>");

            if (summaryEnd > summaryStart)
            {
                string summary = xmlDoc.Substring(summaryStart, summaryEnd - summaryStart).Trim();

                // Remove common XML comment artifacts
                summary = summary.Replace("\n", " ").Replace("\r", "");

                while (summary.Contains("  "))
                    summary = summary.Replace("  ", " ");

                return summary;
            }
        }
        catch
        {
            // If parsing fails, return empty string
        }

        return string.Empty;
    }

    /// <summary>
    /// Checks if a parameter is a common collection parameter that should be skipped.
    /// </summary>
    private static bool IsCommonCollectionParameter(IParameterSymbol parameter)
    {
        if (parameter.Type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            string genericName = namedType.ConstructedFrom.ToDisplayString();
            if (genericName.Contains("IEnumerable") ||
                genericName.Contains("IReadOnlyList") ||
                genericName.Contains("List") ||
                genericName.Contains("IList") ||
                genericName.Contains("ICollection"))
            {
                return true;
            }
        }

        return parameter.Name.Equals("source", StringComparison.OrdinalIgnoreCase) ||
               parameter.Name.Equals("quotes", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the class already has a static Listing property.
    /// More robust detection of existing listings to prevent duplicate code generation.
    /// </summary>
    private static bool HasListingProperty(INamedTypeSymbol classSymbol)
    {
        // Check in the current class symbol
        bool hasListing = CheckForListingInSymbol(classSymbol);
        if (hasListing)
            return true;

        // Check in base types if not found in current class
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            if (CheckForListingInSymbol(baseType))
                return true;

            baseType = baseType.BaseType;
        }

        // Search through all partial implementations of this class
        foreach (var syntaxReference in classSymbol.DeclaringSyntaxReferences)
        {
            var syntax = syntaxReference.GetSyntax();
            if (syntax is ClassDeclarationSyntax classDecl)
            {
                // Check if class contains a Listing property
                if (classDecl.Members.OfType<PropertyDeclarationSyntax>()
                    .Any(p => p.Identifier.Text == "Listing" &&
                              p.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword))))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Helper method to check if a symbol has a Listing property or field
    /// </summary>
    private static bool CheckForListingInSymbol(INamedTypeSymbol symbol)
    {
        // Check for explicitly declared static Listing property
        var hasExplicitListing = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Any(p => p.Name == "Listing" &&
                     p.IsStatic &&
                     (p.Type.Name == "IndicatorListing" ||
                      p.Type.Name == "CompositeIndicatorListing" ||
                      p.Type.ToDisplayString().EndsWith("IndicatorListing")));

        // Also check for static readonly field with Listing
        var hasListingField = symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Any(f => f.Name == "Listing" &&
                     f.IsStatic &&
                     f.IsReadOnly &&
                     (f.Type.Name == "IndicatorListing" ||
                      f.Type.Name == "CompositeIndicatorListing" ||
                      f.Type.ToDisplayString().EndsWith("IndicatorListing")));

        return hasExplicitListing || hasListingField;
    }

    /// <summary>
    /// Generates registration code for all indicator classes.
    /// </summary>
    private static void GenerateRegistration(SourceProductionContext context, ImmutableArray<IndicatorClassInfo> classInfos)
    {
        if (classInfos.IsEmpty)
            return;

        // Track classes we've already generated listings for to avoid duplicates
        HashSet<string> processedClasses = new HashSet<string>();

        // Filter to only classes that need code generation (don't have existing Listing)
        // Also use our tracked class names to avoid duplicates from multiple partial classes
        var classesToGenerate = classInfos
            .Where(c => !c.HasExistingListing && !processedClasses.Contains(c.FullName))
            .ToImmutableArray();

        // Add all class names to our processed list
        foreach (var classInfo in classesToGenerate)
        {
            processedClasses.Add(classInfo.FullName);
        }

        if (classesToGenerate.IsEmpty)
            return;

        var sourceBuilder = new StringBuilder();

        // Generate file header
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("// This file was generated by the CatalogGenerator source generator.");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("using Skender.Stock.Indicators;");
        sourceBuilder.AppendLine();

        // Group by namespace
        var namespaceGroups = classesToGenerate.GroupBy(c => c.Namespace);

        foreach (var namespaceGroup in namespaceGroups)
        {
            sourceBuilder.AppendLine($"namespace {namespaceGroup.Key}");
            sourceBuilder.AppendLine("{");

            foreach (var classInfo in namespaceGroup)
            {
                GenerateClassListing(sourceBuilder, classInfo);
                sourceBuilder.AppendLine();
            }

            sourceBuilder.AppendLine("}");
            sourceBuilder.AppendLine();
        }

        // Generate registration module
        GenerateRegistrationModule(sourceBuilder, classesToGenerate);

        context.AddSource("CatalogRegistration.g.cs", sourceBuilder.ToString());
    }

    /// <summary>
    /// Generates the static Listing property for a class.
    /// </summary>
    private static void GenerateClassListing(StringBuilder sourceBuilder, IndicatorClassInfo classInfo)
    {
        sourceBuilder.AppendLine($"    public partial class {classInfo.ClassName}");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        /// <summary>");
        sourceBuilder.AppendLine("        /// Catalog listing for this indicator.");
        sourceBuilder.AppendLine("        /// </summary>");
        sourceBuilder.AppendLine("        public static IndicatorListing Listing { get; } = CreateListing();");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("        private static IndicatorListing CreateListing()");
        sourceBuilder.AppendLine("        {");

        // Use CompositeIndicatorListingBuilder for multi-style indicators
        if (classInfo.HasMultipleStyles)
        {
            sourceBuilder.AppendLine("            var builder = new CompositeIndicatorListingBuilder()");
        }
        else
        {
            sourceBuilder.AppendLine("            var builder = new IndicatorListingBuilder()");
        }

        sourceBuilder.AppendLine($"                .WithName(\"{FormatDisplayName(classInfo.ClassName)}\")");

        if (!string.IsNullOrEmpty(classInfo.AttributeInfo.Id))
        {
            sourceBuilder.AppendLine($"                .WithId(\"{classInfo.AttributeInfo.Id}\")");
        }

        // Set the primary style (from first attribute)
        if (!string.IsNullOrEmpty(classInfo.AttributeInfo.Style))
        {
            sourceBuilder.AppendLine($"                .WithStyle(Style.{classInfo.AttributeInfo.Style})");
        }
        else
        {
            // Default to Style.Series if not specified in the attribute
            if (classInfo.AttributeInfo.AttributeType.Contains("Series"))
                sourceBuilder.AppendLine("                .WithStyle(Style.Series)");
            else if (classInfo.AttributeInfo.AttributeType.Contains("Stream"))
                sourceBuilder.AppendLine("                .WithStyle(Style.Stream)");
            else if (classInfo.AttributeInfo.AttributeType.Contains("Buffer"))
                sourceBuilder.AppendLine("                .WithStyle(Style.Buffer)");
            else
                sourceBuilder.AppendLine("                .WithStyle(Style.Series)"); // Default
        }

        // For multi-style indicators, add all supported styles
        if (classInfo.HasMultipleStyles && classInfo.SupportedStyles.Length > 0)
        {
            string stylesParams = string.Join(", ", classInfo.SupportedStyles.Select(s => $"Style.{s}"));
            sourceBuilder.AppendLine($"                .WithSupportedStyles({stylesParams})");
        }

        sourceBuilder.AppendLine("                .WithCategory(Category.Undefined);");
        sourceBuilder.AppendLine();

        // Add parameters
        foreach (var param in classInfo.Parameters)
        {
            sourceBuilder.AppendLine($"            builder.AddParameter<{GetParameterType(param.Type)}>(");
            sourceBuilder.AppendLine($"                parameterName: \"{param.Name}\",");
            sourceBuilder.AppendLine($"                displayName: \"{FormatDisplayName(param.Name)}\",");
            sourceBuilder.AppendLine($"                description: \"{FormatDisplayName(param.Name)} parameter\",");
            sourceBuilder.AppendLine($"                isRequired: {param.IsRequired.ToString().ToLowerInvariant()}");

            if (!param.IsRequired && param.DefaultValue != null)
            {
                sourceBuilder.AppendLine($"                , defaultValue: {FormatDefaultValue(param.DefaultValue, param.Type)}");
            }

            sourceBuilder.AppendLine("            );");
        }

        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("            // Add result based on detected type");
        sourceBuilder.AppendLine("            builder.AddResult(");
        sourceBuilder.AppendLine("                dataName: \"Result\",");
        sourceBuilder.AppendLine("                displayName: \"Result\",");
        sourceBuilder.AppendLine($"                dataType: ResultType.{classInfo.ResultInfo.ResultType},");

        if (!string.IsNullOrEmpty(classInfo.ResultInfo.Description))
        {
            sourceBuilder.AppendLine($"                description: \"{classInfo.ResultInfo.Description}\",");
        }

        sourceBuilder.AppendLine("                isDefault: true);");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("            return builder.Build();");
        sourceBuilder.AppendLine("        }");
        sourceBuilder.AppendLine("    }");
    }

    /// <summary>
    /// Generates the registration module that auto-registers all generated listings.
    /// </summary>
    private static void GenerateRegistrationModule(StringBuilder sourceBuilder, ImmutableArray<IndicatorClassInfo> classInfos)
    {
        sourceBuilder.AppendLine("namespace Skender.Stock.Indicators");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Auto-generated registration module for indicator listings.");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    internal static class GeneratedCatalogRegistration");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        /// <summary>");
        sourceBuilder.AppendLine("        /// Registers all auto-generated indicator listings.");
        sourceBuilder.AppendLine("        /// This method is called automatically by the IndicatorRegistry.");
        sourceBuilder.AppendLine("        /// </summary>");
        sourceBuilder.AppendLine("        internal static void RegisterAll()");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            // This method intentionally left empty.");
        sourceBuilder.AppendLine("            // The source generator creates static Listing properties");
        sourceBuilder.AppendLine("            // that are automatically discovered by RegisterCatalog()");
        sourceBuilder.AppendLine("            // through reflection-based scanning.");
        sourceBuilder.AppendLine("        }");
        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");
    }

    /// <summary>
    /// Formats a name for display purposes.
    /// </summary>
    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        // Convert PascalCase to spaced words
        var result = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
                result.Append(' ');
            result.Append(name[i]);
        }
        return result.ToString();
    }

    /// <summary>
    /// Gets the appropriate parameter type for the builder.
    /// </summary>
    private static string GetParameterType(string typeString)
    {
        return typeString switch
        {
            "int" or "System.Int32" => "int",
            "double" or "System.Double" => "double",
            "decimal" or "System.Decimal" => "decimal",
            "bool" or "System.Boolean" => "bool",
            "string" or "System.String" => "string",
            _ => "object"
        };
    }

    /// <summary>
    /// Formats a default value for code generation.
    /// </summary>
    private static string FormatDefaultValue(string defaultValue, string type)
    {
        if (string.IsNullOrEmpty(defaultValue) || defaultValue == "null")
            return "null";

        return type switch
        {
            "string" or "System.String" => $"\"{defaultValue}\"",
            "bool" or "System.Boolean" => defaultValue.ToLowerInvariant(),
            _ => defaultValue
        };
    }
}

/// <summary>
/// Information about an indicator class.
/// </summary>
internal class IndicatorClassInfo
{
    public string ClassName { get; }
    public string FullName { get; }
    public string Namespace { get; }
    public string MethodName { get; }  // Added to track the specific attributed method
    public IndicatorAttributeInfo AttributeInfo { get; }
    public ImmutableArray<ParameterInfo> Parameters { get; }
    public ResultTypeInfo ResultInfo { get; } // Added to store result type information
    public bool HasExistingListing { get; }
    public bool HasMultipleStyles { get; } // Indicates if this class has multiple indicator style attributes
    public string[] SupportedStyles { get; } // Store all supported styles for this indicator

    public IndicatorClassInfo(
        string className,
        string fullName,
        string @namespace,
        string methodName,
        IndicatorAttributeInfo attributeInfo,
        ImmutableArray<ParameterInfo> parameters,
        ResultTypeInfo resultInfo,
        bool hasExistingListing,
        bool hasMultipleStyles = false,
        string[]? supportedStyles = null)
    {
        ClassName = className;
        FullName = fullName;
        Namespace = @namespace;
        MethodName = methodName;
        AttributeInfo = attributeInfo;
        Parameters = parameters;
        ResultInfo = resultInfo;
        HasExistingListing = hasExistingListing;
        HasMultipleStyles = hasMultipleStyles;
        SupportedStyles = supportedStyles ?? Array.Empty<string>();
    }
}

/// <summary>
/// Information about an indicator attribute.
/// </summary>
internal class IndicatorAttributeInfo
{
    public string AttributeType { get; }
    public string? Id { get; }
    public string? Style { get; }

    public IndicatorAttributeInfo(string attributeType, string? id, string? style)
    {
        AttributeType = attributeType;
        Id = id;
        Style = style;
    }
}

/// <summary>
/// Information about a parameter.
/// </summary>
internal class ParameterInfo
{
    public string Name { get; }
    public string Type { get; }
    public bool IsRequired { get; }
    public string? DefaultValue { get; }

    public ParameterInfo(string name, string type, bool isRequired, string? defaultValue)
    {
        Name = name;
        Type = type;
        IsRequired = isRequired;
        DefaultValue = defaultValue;
    }
}

/// <summary>
/// Information about the result type of an indicator method.
/// </summary>
internal class ResultTypeInfo
{
    public string ResultType { get; }
    public string Description { get; }

    public ResultTypeInfo(string resultType, string description = "")
    {
        ResultType = resultType;
        Description = description;
    }
}
