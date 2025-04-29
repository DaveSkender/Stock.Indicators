using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis.Text;

namespace Indicators.Catalog.Generator;

/// <summary>
/// Source generator for catalog information about indicators from attributes.
/// </summary>
[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    // Get version dynamically - uses compile-time constant in CI or timestamp locally
    private static string GetGeneratorVersion() =>
#if VERSIONED_BUILD
        // This value will be replaced during CI build
        return "#{PACKAGE_VERSION}#";
#else
        // For local builds, use a timestamp-based version
        DateTime.Now.ToString("yyyy.MM.dd-HH:mm:ss.fff");
#endif


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the syntax providers for different attribute types
        IncrementalValuesProvider<SyntaxNode> seriesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "Series", "SeriesAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Stream attributes on methods
        IncrementalValuesProvider<SyntaxNode> streamProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "StreamHub", "StreamAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Buffer attributes on constructors
        IncrementalValuesProvider<SyntaxNode> bufferProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForAttribute(s, "Buffer", "BufferAttribute"),
                transform: static (ctx, _) => ctx.Node)
            .Where(static m => m is not null)!;

        // Combine all providers with compilation
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledSeriesProvider = seriesProvider.Collect();
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledStreamProvider = streamProvider.Collect();
        IncrementalValueProvider<ImmutableArray<SyntaxNode>> compiledBufferProvider = bufferProvider.Collect();

        IncrementalValueProvider<(((
            Compilation Left, ImmutableArray<SyntaxNode> Right) Left,
            ImmutableArray<SyntaxNode> Right) Left,
            ImmutableArray<SyntaxNode> Right)> combinedProvider
            = context.CompilationProvider
             .Combine(compiledSeriesProvider)
             .Combine(compiledStreamProvider)
             .Combine(compiledBufferProvider);

        // Register the source output generation
        context.RegisterSourceOutput(
            combinedProvider,
            (spc, tuple) => Execute(
                spc,
                tuple.Left.Left.Left,
                tuple.Left.Left.Right,
                tuple.Left.Right,
                tuple.Right));
    }

    private static bool IsCandidateForAttribute(
        SyntaxNode node, params string[] attributeNames)
        => node is MemberDeclarationSyntax member
            && member.AttributeLists.Count != 0
            && member.AttributeLists.Any(
                al => al.Attributes.Any(
                    a => attributeNames.Any(
                        attr => a.Name.ToString().Contains(attr))));

    private static void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<SyntaxNode> seriesNodes,
        ImmutableArray<SyntaxNode> streamNodes,
        ImmutableArray<SyntaxNode> bufferNodes)
    {
        // Get required symbol references
        Dictionary<string, INamedTypeSymbol?> requiredSymbols = GetRequiredSymbols(compilation);

        if (!AreRequiredSymbolsAvailable(requiredSymbols))
        {
            return;
        }

        List<IndicatorInfo> indicators = [];
        HashSet<string> processedIds = [];

        // Process nodes for each attribute type
        ProcessNodes(compilation, "Series", seriesNodes, requiredSymbols, indicators, processedIds);
        ProcessNodes(compilation, "Stream", streamNodes, requiredSymbols, indicators, processedIds);
        ProcessNodes(compilation, "Buffer", bufferNodes, requiredSymbols, indicators, processedIds);

        // Validate indicator UIIDs are unique before generating code
        ValidateUniqueUIIDs(indicators, context);

        // Generate catalog class
        string sourceCode = GenerateCatalogClass(indicators);
        context.AddSource("Catalog.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static void ValidateUniqueUIIDs(List<IndicatorInfo> indicators, SourceProductionContext context)
    {
        List<string> duplicateUIIDs = indicators
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateUIIDs.Count != 0)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "IND004",
                        "Duplicate UIIDs detected",
                        "The following UIIDs are used more than once: {0}",
                        "Catalog",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    Location.None,
                    string.Join(", ", duplicateUIIDs)));
        }
    }

    private static Dictionary<string, INamedTypeSymbol?> GetRequiredSymbols(Compilation compilation)
    {
        const string srcNamespace = "Skender.Stock.Indicators";

        return new() {
            ["SeriesAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.SeriesAttribute"),
            ["StreamAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.StreamAttribute"),
            ["BufferAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.BufferAttribute"),
            ["ParamAttributeBase"] = compilation.GetTypeByMetadataName($"{srcNamespace}.ParamAttribute`1"),
            ["Category"] = compilation.GetTypeByMetadataName($"{srcNamespace}.Category"),
            ["ChartType"] = compilation.GetTypeByMetadataName($"{srcNamespace}.ChartType")
        };
    }

    private static bool AreRequiredSymbolsAvailable(
        Dictionary<string, INamedTypeSymbol?> symbols)
        => symbols.Values.All(symbol => symbol != null);

    private static void ProcessNodes(
        Compilation compilation,
        string attributeType,
        ImmutableArray<SyntaxNode> nodes,
        Dictionary<string, INamedTypeSymbol?> symbols,
        List<IndicatorInfo> indicators,
        HashSet<string> processedIds)
    {
        foreach (SyntaxNode node in nodes)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(node.SyntaxTree);
            ISymbol? symbol = semanticModel.GetDeclaredSymbol(node);

            if (symbol is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in methodSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeSymbol = symbols[attributeType + "Attribute"];

                if (attributeSymbol == null ||
                    !attributeData.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                string id = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(id) || !processedIds.Add(id))
                {
                    continue;
                }

                string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;

                // Convert category from enum to string
                object? categoryValue = attributeData.ConstructorArguments[2].Value;
                string category = categoryValue != null ?
                    GetEnumFieldName(symbols["Category"]!, Convert.ToInt32(categoryValue)) : string.Empty;

                // Convert chartType from enum to string
                object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                string chartType = chartTypeValue != null ?
                    GetEnumFieldName(symbols["ChartType"]!, Convert.ToInt32(chartTypeValue)) : string.Empty;

                // Extract legendOverride if provided
                string? legendOverride = null;
                if (attributeData.ConstructorArguments.Length >= 5 && attributeData.ConstructorArguments[4].Value != null)
                {
                    legendOverride = attributeData.ConstructorArguments[4].Value?.ToString();
                }

                // Process parameters
                List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, symbols["ParamAttributeBase"]);

                // Create indicator info
                indicators.Add(new IndicatorInfo(
                    Id: id,
                    Name: name,
                    Type: attributeType,
                    ContainingType: methodSymbol.ContainingType.Name,
                    MemberName: methodSymbol.Name == ".ctor" ? "Constructor" : methodSymbol.Name,
                    Category: category,
                    ChartType: chartType,
                    LegendOverride: legendOverride,
                    Parameters: parameters));
            }
        }
    }

    /// <summary>
    /// Retrieves the name of an enum field from its integer value.
    /// </summary>
    private static string GetEnumFieldName(INamedTypeSymbol enumType, int value)
    {
        foreach (IFieldSymbol member in enumType.GetMembers().OfType<IFieldSymbol>())
        {
            if (member.HasConstantValue && Convert.ToInt32(member.ConstantValue) == value)
            {
                return member.Name;
            }
        }

        return string.Empty;
    }

    private static List<ParameterInfo> GetMethodParameters(
        IMethodSymbol methodSymbol,
        INamedTypeSymbol? paramAttributeBaseSymbol)
    {
        List<ParameterInfo> parameters = [];

        // If paramAttributeBaseSymbol is null, we can't identify parameters
        if (paramAttributeBaseSymbol is null)
        {
            return parameters;
        }

        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            // Find attributes that derive from ParamAttribute<T>
            AttributeData? paramAttribute = null;
            foreach (AttributeData attr in parameter.GetAttributes())
            {
                if (IsParamAttributeOrDerived(attr.AttributeClass, paramAttributeBaseSymbol))
                {
                    paramAttribute = attr;
                    break;
                }
            }

            if (paramAttribute == null)
            {
                continue; // Skip parameters without a compatible ParamAttribute
            }

            // Get attribute type to determine how to extract information
            INamedTypeSymbol? attrClass = paramAttribute.AttributeClass;
            if (attrClass == null)
            {
                continue;
            }

            string attributeClassName = attrClass.Name;
            string displayName = GetDisplayName(paramAttribute, parameter.Name);

            // Extract parameter values based on attribute type
            ExtractAttributeValues(
                paramAttribute,
                attrClass,
                out string dataType,
                out double defaultValue,
                out double minValue,
                out double maxValue,
                out string? enumTypeName);

            parameters.Add(new ParameterInfo(
                parameter.Name,
                displayName,
                dataType,
                defaultValue,
                minValue,
                maxValue,
                enumTypeName));
        }

        return parameters;
    }

    private static void ExtractAttributeValues(
        AttributeData attribute,
        INamedTypeSymbol attributeClass,
        out string dataType,
        out double defaultValue,
        out double minValue,
        out double maxValue,
        out string? enumTypeName)
    {
        string attributeClassName = attributeClass.Name;
        enumTypeName = null;

        // Handle different attribute types based on class name
        if (attributeClassName == "ParamBoolAttribute")
        {
            // Handle ParamBoolAttribute
            dataType = "boolean";
            defaultValue = ExtractBooleanDefaultValue(attribute) ? 1 : 0;
            minValue = 0;
            maxValue = 1;
        }
        else if (attributeClassName.StartsWith("ParamEnum"))
        {
            // Handle ParamEnumAttribute<T>
            dataType = "enum";
            enumTypeName = ExtractEnumTypeName(attributeClass);

            // For enum types, get actual min/max values from enum
            // These are already calculated in the ParamEnumAttribute constructor
            (int min, int max) = GetEnumMinMaxValues(attributeClass);
            defaultValue = ExtractEnumDefaultValue(attribute);
            minValue = min;
            maxValue = max;
        }
        else // ParamNumAttribute<T>
        {
            // Handle ParamNumAttribute<T>
            dataType = DetermineNumericDataType(attributeClass);
            ExtractNumericValues(attribute, out minValue, out maxValue, out defaultValue);
        }

        // Ensure default is within min/max bounds for numeric types
        if (dataType is "number" or "int")
        {
            defaultValue = Math.Min(Math.Max(defaultValue, minValue), maxValue);
        }
    }

    private static string GetDisplayName(AttributeData attribute, string defaultName)
    {
        // Try to get display name from constructor argument (first argument)
        if (attribute.ConstructorArguments.Length > 0 &&
            attribute.ConstructorArguments[0].Value is string displayName)
        {
            return displayName;
        }

        // Try named argument
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DisplayName" && namedArg.Value.Value is string name)
            {
                return name;
            }
        }

        return defaultName;
    }

    private static string DetermineNumericDataType(INamedTypeSymbol attributeClass)
    {
        // Check the generic type argument for numeric attributes
        if (attributeClass.TypeArguments.Length > 0)
        {
            ITypeSymbol typeArg = attributeClass.TypeArguments[0];
            if (typeArg.SpecialType == SpecialType.System_Int32 ||
                typeArg.Name == "Int32")
            {
                return "int";
            }
        }

        return "number"; // Default to number for decimal, double, etc.
    }

    private static void ExtractNumericValues(
        AttributeData attribute,
        out double minValue,
        out double maxValue,
        out double defaultValue)
    {
        defaultValue = 0;
        minValue = 0;
        maxValue = 100;

        // Try to extract from constructor arguments
        if (attribute.ConstructorArguments.Length >= 4)
        {
            // Format expected: (displayName, defaultValue, minValue, maxValue)
            if (attribute.ConstructorArguments[1].Value != null)
            {
                defaultValue = Convert.ToDouble(attribute.ConstructorArguments[1].Value);
            }

            if (attribute.ConstructorArguments[2].Value != null)
            {
                minValue = Convert.ToDouble(attribute.ConstructorArguments[2].Value);
            }

            if (attribute.ConstructorArguments[3].Value != null)
            {
                maxValue = Convert.ToDouble(attribute.ConstructorArguments[3].Value);
            }
        }

        // Or try named arguments
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value != null)
            {
                defaultValue = Convert.ToDouble(namedArg.Value.Value);
            }
            else if (namedArg.Key == "MinValue" && namedArg.Value.Value != null)
            {
                minValue = Convert.ToDouble(namedArg.Value.Value);
            }
            else if (namedArg.Key == "MaxValue" && namedArg.Value.Value != null)
            {
                maxValue = Convert.ToDouble(namedArg.Value.Value);
            }
        }
    }

    private static bool ExtractBooleanDefaultValue(AttributeData attribute)
    {
        // Try to get default value from constructor argument
        if (attribute.ConstructorArguments.Length >= 2 &&
            attribute.ConstructorArguments[1].Value is bool defaultValue)
        {
            return defaultValue;
        }

        // Try named argument
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value is bool boolVal)
            {
                return boolVal;
            }
        }

        return false; // Default if not found
    }

    private static string? ExtractEnumTypeName(INamedTypeSymbol attributeClass)
    {
        // Get the enum type from the generic argument
        if (attributeClass.TypeArguments.Length > 0)
        {
            ITypeSymbol typeArg = attributeClass.TypeArguments[0];
            if (typeArg.TypeKind == TypeKind.Enum)
            {
                return typeArg.Name;
            }
        }

        return null;
    }

    private static int ExtractEnumDefaultValue(AttributeData attribute)
    {
        // Try to get default enum value from constructor argument
        if (attribute.ConstructorArguments.Length >= 2 &&
            attribute.ConstructorArguments[1].Value != null)
        {
            return Convert.ToInt32(attribute.ConstructorArguments[1].Value);
        }

        // Try named argument
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value != null)
            {
                return Convert.ToInt32(namedArg.Value.Value);
            }
        }

        return 0; // Default if not found
    }

    private static (int min, int max) GetEnumMinMaxValues(INamedTypeSymbol attributeClass)
    {
        // Default values if we can't determine actual values
        int minValue = 0;
        int maxValue = 0;

        // Get the enum type from the generic argument
        if (attributeClass.TypeArguments.Length > 0)
        {
            ITypeSymbol typeArg = attributeClass.TypeArguments[0];
            if (typeArg.TypeKind == TypeKind.Enum)
            {
                // Get all enum values
                List<int> enumValues = [];

                foreach (ISymbol member in typeArg.GetMembers())
                {
                    if (member is IFieldSymbol field && field.HasConstantValue)
                    {
                        enumValues.Add(Convert.ToInt32(field.ConstantValue));
                    }
                }

                if (enumValues.Count > 0)
                {
                    minValue = enumValues.Min();
                    maxValue = enumValues.Max();
                }
            }
        }

        return (minValue, maxValue);
    }

    /// <summary>
    /// Determines if an attribute class is derived from ParamAttribute<T>.
    /// </summary>
    private static bool IsParamAttributeOrDerived(INamedTypeSymbol? attributeClass, INamedTypeSymbol? baseParamAttributeSymbol)
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

    private static string GenerateCatalogClass(List<IndicatorInfo> indicators)
    {
        StringBuilder sourceBuilder = new();

        // Get the version string
        string version = GetGeneratorVersion();

        // File header and namespaces
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("using System.Collections.Generic;");
        sourceBuilder.AppendLine("using System.Linq;");
        sourceBuilder.AppendLine("using System.CodeDom.Compiler;");
        sourceBuilder.AppendLine("");
        sourceBuilder.AppendLine("namespace Skender.Stock.Indicators;");
        sourceBuilder.AppendLine("");
        sourceBuilder.AppendLine("/// <summary>");
        sourceBuilder.AppendLine("/// Auto-generated catalog of all indicators in the library");
        sourceBuilder.AppendLine("/// </summary>");
        sourceBuilder.AppendLine($"[GeneratedCode(\"Indicators.Catalog.Generator\", \"{version}\")]");
        sourceBuilder.AppendLine("public static partial class Catalog");
        sourceBuilder.AppendLine("{");

        // Generate the GeneratedListings property implementation
        sourceBuilder.AppendLine("""
            /// <summary>
            /// Gets all indicator listings in the catalog (auto-generated)
            /// </summary>
            private static partial IReadOnlyList<IndicatorListing> GeneratedListings
            {
                get
                {
                    var indicators = new List<IndicatorListing>
                    {
        """);

        // Add all found indicators to the collection
        foreach (IndicatorInfo? indicator in indicators.OrderBy(i => i.Name))
        {
            AppendIndicatorListing(sourceBuilder, indicator);
        }

        // Finalize class content
        sourceBuilder.AppendLine("""
                    };

                    return indicators;
                }
            }
        }
        """);

        return sourceBuilder.ToString();
    }

    private static void AppendIndicatorListing(StringBuilder sourceBuilder, IndicatorInfo indicator)
    {
        // Use the indicator name directly without appending the type
        string displayName = indicator.Name;

        // Build tooltip template based on parameters or use the legend override
        string legendTemplate = indicator.Parameters.Count > 0
                ? $"{indicator.Id}("
                            + string.Join(",",
                                Enumerable
                                    .Range(1, indicator.Parameters.Count)
                                    .Select(i => $"[P{i}]")
                                ) + ")"
                : indicator.Id;

        // Use the legend override if provided
        if (indicator.LegendOverride != null
         && indicator.LegendOverride != string.Empty)
        {
            legendTemplate = indicator.LegendOverride;
        }

        sourceBuilder.AppendLine($$"""
                new IndicatorListing
                {
                    Name = "{{displayName}}",
                    Uiid = "{{indicator.Id}}",
                    Category = Category.{{indicator.Category}},
                    ChartType = ChartType.{{indicator.ChartType}},
                    Order = Order.Front,
                    ChartConfig = null,
                    LegendTemplate = "{{legendTemplate}}",
        """);

        // Add parameters
        if (indicator.Parameters.Count > 0)
        {
            sourceBuilder.AppendLine("""
                    Parameters = new List<IndicatorParamConfig>
                    {
            """);

            foreach (ParameterInfo param in indicator.Parameters)
            {
                AppendParameterConfig(sourceBuilder, param);
            }

            sourceBuilder.AppendLine("                    },");
        }
        else
        {
            sourceBuilder.AppendLine("                    Parameters = new List<IndicatorParamConfig>(),");
        }

        // Add result configs
        AppendResultConfig(sourceBuilder, indicator.Name, indicator.Id, legendTemplate);

        sourceBuilder.AppendLine("                },");
    }

    private static void AppendParameterConfig(StringBuilder sourceBuilder, ParameterInfo param)
    {
        // Format numeric values without 'd' suffix
        string minValueStr = FormatNumericValue(param.MinValue);
        string maxValueStr = FormatNumericValue(param.MaxValue);
        string defaultValueStr = FormatNumericValue(param.DefaultValue);

        sourceBuilder.AppendLine($$"""
                        new IndicatorParamConfig
                        {
                            ParamName = "{{param.Name}}",
                            DisplayName = "{{param.DisplayName}}",
                            DataType = "{{param.DataType}}",
                            DefaultValue = {{defaultValueStr}},
                            Minimum = {{minValueStr}},
                            Maximum = {{maxValueStr}}
                        },
        """);
    }

    private static void AppendResultConfig(
        StringBuilder sourceBuilder, string name, string id, string tooltipTemplate)
        => sourceBuilder.AppendLine($$"""
                    Results = new List<IndicatorResultConfig>
                    {
                        new IndicatorResultConfig
                        {
                            DataName = "{{id.ToLowerInvariant()}}",
                            DisplayName = "{{name}}",
                            TooltipTemplate = "{{tooltipTemplate}}",
                            DataType = "number",
                            DefaultColor = ChartColors.StandardBlue,
                            LineType = "solid",
                            LineWidth = 2,
                            Stack = null,
                            Fill = null
                        }
                    }
        """);

    /// <summary>
    /// Formats a numeric value as a string without the 'd' suffix.
    /// </summary>
    private static string FormatNumericValue(double value)
    {
        // Handle special values like infinity
        if (double.IsPositiveInfinity(value))
        {
            return "double.PositiveInfinity";
        }
        else if (double.IsNegativeInfinity(value))
        {
            return "double.NegativeInfinity";
        }

        // Use invariant culture to avoid locale-specific formatting issues
        return value.ToString("G", CultureInfo.InvariantCulture);
    }

    private sealed record IndicatorInfo(
        string Id,
        string Name,
        string Type,
        string ContainingType,
        string MemberName,
        string Category,
        string ChartType,
        string? LegendOverride,
        List<ParameterInfo> Parameters);

    private sealed record ParameterInfo(
        string Name,
        string DisplayName,
        string DataType,
        double DefaultValue,
        double MinValue,
        double MaxValue,
        string? EnumType = null,
        Dictionary<string, int>? EnumValues = null);
}
