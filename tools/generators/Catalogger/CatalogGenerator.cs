using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

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

        // Quick check: Does class have static methods with attributes?
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

        // Check each method for indicator attributes
        foreach (var method in methods)
        {
            var attributeInfo = ExtractIndicatorAttributeFromMethod(method);
            if (attributeInfo != null)
            {
                // Extract parameter information from the attributed method
                var parameters = ExtractParameterInfoFromMethod(method);

                // Check if class already has a Listing property
                bool hasExistingListing = HasListingProperty(classSymbol);

                return new IndicatorClassInfo(
                    classSymbol.Name,
                    classSymbol.ToDisplayString(),
                    classSymbol.ContainingNamespace.ToDisplayString(),
                    method.Name,
                    attributeInfo,
                    parameters,
                    hasExistingListing);
            }
        }

        return null;
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
    /// </summary>
    private static bool HasListingProperty(INamedTypeSymbol classSymbol)
    {
        return classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Any(p => p.Name == "Listing" &&
                     p.IsStatic &&
                     p.Type.Name == "IndicatorListing");
    }

    /// <summary>
    /// Generates registration code for all indicator classes.
    /// </summary>
    private static void GenerateRegistration(SourceProductionContext context, ImmutableArray<IndicatorClassInfo> classInfos)
    {
        if (classInfos.IsEmpty)
            return;

        // Filter to only classes that need code generation (don't have existing Listing)
        var classesToGenerate = classInfos.Where(c => !c.HasExistingListing).ToImmutableArray();

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
        sourceBuilder.AppendLine("            var builder = new IndicatorListingBuilder()");
        sourceBuilder.AppendLine($"                .WithName(\"{FormatDisplayName(classInfo.ClassName)}\")");

        if (!string.IsNullOrEmpty(classInfo.AttributeInfo.Id))
        {
            sourceBuilder.AppendLine($"                .WithId(\"{classInfo.AttributeInfo.Id}\")");
        }

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
        sourceBuilder.AppendLine("            // Add default result");
        sourceBuilder.AppendLine("            builder.AddResult(");
        sourceBuilder.AppendLine("                dataName: \"Result\",");
        sourceBuilder.AppendLine("                displayName: \"Result\",");
        sourceBuilder.AppendLine("                dataType: ResultType.Default,");
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
    public bool HasExistingListing { get; }

    public IndicatorClassInfo(
        string className,
        string fullName,
        string @namespace,
        string methodName,
        IndicatorAttributeInfo attributeInfo,
        ImmutableArray<ParameterInfo> parameters,
        bool hasExistingListing)
    {
        ClassName = className;
        FullName = fullName;
        Namespace = @namespace;
        MethodName = methodName;
        AttributeInfo = attributeInfo;
        Parameters = parameters;
        HasExistingListing = hasExistingListing;
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
