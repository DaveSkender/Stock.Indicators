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
        ProcessNodes(context, compilation, "Series", seriesNodes, requiredSymbols, indicators, processedIds);
        ProcessNodes(context, compilation, "Stream", streamNodes, requiredSymbols, indicators, processedIds);
        ProcessNodes(context, compilation, "Buffer", bufferNodes, requiredSymbols, indicators, processedIds);

        // Validate indicator UIIDs are unique before generating code
        Validate(context, indicators);

        // Generate catalog class
        string sourceCode = GenerateCatalogClass(indicators);
        context.AddSource("Catalog.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static void Validate(SourceProductionContext context, List<IndicatorInfo> indicators)
    {
        List<string> duplicateUIIDs = indicators
            .GroupBy(x => x.Uiid)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateUIIDs.Count != 0)
        {
            ReportIND901_DuplicateListings(context, duplicateUIIDs);
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
        SourceProductionContext context,
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

                if (attributeSymbol == null
                || !attributeData.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default))
                {
                    continue;
                }

                string uiid = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(uiid) || !processedIds.Add(uiid))
                {
                    continue;
                }

                string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;

                // Convert category from enum to string
                object? categoryValue = attributeData.ConstructorArguments[2].Value;
                string category = categoryValue != null
                    ? GetEnumFieldName(symbols["Category"]!, Convert.ToInt32(categoryValue))
                    : string.Empty;

                // Convert chartType from enum to string
                object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                string chartType = chartTypeValue != null
                    ? GetEnumFieldName(symbols["ChartType"]!, Convert.ToInt32(chartTypeValue))
                    : string.Empty;

                // Extract legendOverride, if provided
                string? legendOverride = null;
                if (attributeData.ConstructorArguments.Length >= 5
                 && attributeData.ConstructorArguments[4].Value != null)
                {
                    legendOverride = attributeData.ConstructorArguments[4].Value?.ToString();
                }

                // Process parameters
                List<ParameterInfo> parameters = GetMethodParameters(
                    context: context,
                    methodSymbol: methodSymbol,
                    paramAttributeBaseSymbol: symbols["ParamAttributeBase"]);

                // Create indicator info
                indicators.Add(new IndicatorInfo(
                    Uiid: uiid,
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
        SourceProductionContext context,
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
                context: context,
                attribute: paramAttribute,
                attributeClass: attrClass,
                dataType: out string dataType,
                defaultValue: out double? defaultValue,
                minValue: out double? minValue,
                maxValue: out double? maxValue,
                enumTypeName: out string? enumTypeName,
                enumValues: out Dictionary<int, string>? enumValues);

            parameters.Add(new ParameterInfo(
                Name: parameter.Name,
                DisplayName: displayName,
                DataType: dataType,
                DefaultValue: defaultValue,
                MinValue: minValue,
                MaxValue: maxValue,
                EnumType: enumTypeName,
                EnumValues: enumValues));
        }

        return parameters;
    }

    private static void ExtractAttributeValues(
        SourceProductionContext context,
        AttributeData attribute,
        INamedTypeSymbol attributeClass,
        out string dataType,
        out double? defaultValue,
        out double? minValue,
        out double? maxValue,
        out string? enumTypeName,
        out Dictionary<int, string>? enumValues)
    {
        string attributeClassName = attributeClass.Name;

        minValue = null;
        maxValue = null;
        enumTypeName = null;
        enumValues = null;

        // Handle different attribute types based on class name

        if (attributeClassName == "ParamBoolAttribute")
        {
            // Handle ParamBoolAttribute
            dataType = "boolean";
            defaultValue = ExtractBooleanDefaultValue(attribute) ? 1 : 0;
            // Boolean values are constrained between 0 and 1
            minValue = 0;
            maxValue = 1;
        }

        else if (attributeClassName.StartsWith("ParamEnum"))
        {
            // Handle ParamEnumAttribute<T>
            dataType = "enum";
            enumTypeName = ExtractEnumTypeName(attributeClass: attributeClass);
            defaultValue = ExtractEnumDefaultValue(attribute: attribute);

            // Get the enum type and extract its values
            ITypeSymbol? enumType = attributeClass.TypeArguments.Length > 0
                ? attributeClass.TypeArguments[0]
                : null;

            enumValues = GetEnumValues(enumType);

            // Min/max if we have enum values (not required for enum types)
            if (enumValues != null && enumValues.Count > 0)
            {
                List<int> values = enumValues.Keys.ToList();

                if (values.Count > 0)
                {
                    minValue = values.Min();
                    maxValue = values.Max();
                }
            }
        }

        else if (attributeClassName.StartsWith("ParamNum"))
        {
            // Handle ParamNumAttribute<T>
            dataType = DetermineNumericDataType(attributeClass: attributeClass);

            ExtractNumericValues(
                attribute: attribute,
                defaultValue: out defaultValue,
                minValue: out minValue,
                maxValue: out maxValue);
        }

        else
        {
            // failover to default numeric type
            dataType = "number"; // default to number
            defaultValue = null;
            minValue = null;
            maxValue = null;

            throw new InvalidOperationException(
                $"Unsupported attribute type: '{attributeClassName}'. "
               + "Please ensure the attribute is derived from ParamAttribute<T>.");
        }

        // Ensure default is within min/max bounds for numeric types
        if (dataType is "number" or "int" && defaultValue.HasValue && minValue.HasValue && maxValue.HasValue)
        {
            if (defaultValue < minValue || defaultValue > maxValue)
            {
                ReportIND902_InvalidDefaultValue(context, defaultValue, minValue, maxValue);
            }
        }
    }

    private static Dictionary<int, string>? GetEnumValues(ITypeSymbol? enumType)
    {
        if (enumType == null || enumType.TypeKind != TypeKind.Enum)
        {
            return null;
        }

        Dictionary<int, string> enumValues = [];

        foreach (ISymbol member in enumType.GetMembers())
        {
            if (member is IFieldSymbol field && field.HasConstantValue && !field.IsStatic)
            {
                int value = Convert.ToInt32(field.ConstantValue);
                enumValues[value] = field.Name;
            }
        }

        return enumValues.Count > 0 ? enumValues : null;
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

            if (typeArg.SpecialType == SpecialType.System_Int32 || typeArg.Name == "Int32")
            {
                return "int";
            }
        }

        return "number"; // Default to number for decimal, double, etc.
    }

    private static void ExtractNumericValues(
        AttributeData attribute,
        out double? defaultValue,
        out double? minValue,
        out double? maxValue)
    {
        minValue = null;
        maxValue = null;
        defaultValue = null;

        // Try to extract from constructor arguments
        if (attribute.ConstructorArguments.Length >= 4)
        {
            // Format expected: (displayName, defaultValue, minValue, maxValue)
            if (attribute.ConstructorArguments[1].Value != null)
            {
                defaultValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[1].Value,
                    provider: CultureInfo.InvariantCulture);
            }

            if (attribute.ConstructorArguments[2].Value != null)
            {
                minValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[2].Value,
                    provider: CultureInfo.InvariantCulture);
            }

            if (attribute.ConstructorArguments[3].Value != null)
            {
                maxValue = Convert.ToDouble(
                    value: attribute.ConstructorArguments[3].Value,
                    provider: CultureInfo.InvariantCulture);
            }
        }

        // Or try named arguments (preferred)
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Value.Value is null)
            {
                continue;
            }

            switch (namedArg.Key)
            {
                case "DefaultValue":
                    defaultValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;

                case "MinValue":
                    minValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;

                case "MaxValue":
                    maxValue = Convert.ToDouble(
                        value: namedArg.Value.Value,
                        provider: CultureInfo.InvariantCulture);
                    break;
            }
        }
    }

    private static bool ExtractBooleanDefaultValue(AttributeData attribute)
    {
        // Try to get default value from constructor argument
        if (attribute.ConstructorArguments.Length >= 2
         && attribute.ConstructorArguments[1].Value is bool defaultValue)
        {
            return defaultValue;
        }

        // Try named argument (preferred)
        foreach (KeyValuePair<string, TypedConstant> namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "DefaultValue" && namedArg.Value.Value is bool boolVal)
            {
                return boolVal;
            }
        }

        return false; // if not found
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
        if (attribute.ConstructorArguments.Length >= 2
         && attribute.ConstructorArguments[1].Value != null)
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

    /// <summary>
    /// Determines if an attribute class is derived from ParamAttribute<T>.
    /// </summary>
    private static bool IsParamAttributeOrDerived(
        INamedTypeSymbol? attributeClass,
        INamedTypeSymbol? baseParamAttrSymbol)
    {
        if (attributeClass == null || baseParamAttrSymbol == null)
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
                if (SymbolEqualityComparer.Default.Equals(currentType, baseParamAttrSymbol))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }
        }

        // Check non-generic derived types (like ParamBoolAttribute)
        return attributeClass.BaseType != null
            && IsParamAttributeOrDerived(
                attributeClass: attributeClass.BaseType,
                baseParamAttrSymbol: baseParamAttrSymbol);
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
                get {
                    List<IndicatorListing> indicators = new()
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

    private static void AppendIndicatorListing(
        StringBuilder sourceBuilder,
        IndicatorInfo indicator)
    {
        // Use the indicator name directly without appending the type
        string displayName = indicator.Name;

        // Build tooltip template based on parameters or use the legend override
        string legendTemplate = indicator.Parameters.Count > 0
                ? $"{indicator.Uiid}("
                            + string.Join(",",
                                Enumerable
                                    .Range(1, indicator.Parameters.Count)
                                    .Select(i => $"[P{i}]")
                                ) + ")"
                : indicator.Uiid;

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
                        Uiid = "{{indicator.Uiid}}",
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

            sourceBuilder.AppendLine("                },");
        }
        else
        {
            sourceBuilder.AppendLine("                Parameters = new List<IndicatorParamConfig>(),");
        }

        // Add result configs
        AppendResultConfig(sourceBuilder, indicator.Name, indicator.Uiid, legendTemplate);

        sourceBuilder.AppendLine("            },");
    }

    private static void AppendParameterConfig(StringBuilder sourceBuilder, ParameterInfo param)
    {
        // Format numeric values as strings with null support
        string minValueStr = param.MinValue.HasValue ? FormatNumericValue(param.MinValue.Value) : "null";
        string maxValueStr = param.MaxValue.HasValue ? FormatNumericValue(param.MaxValue.Value) : "null";
        string defaultValueStr = param.DefaultValue.HasValue ? FormatNumericValue(param.DefaultValue.Value) : "null";

        sourceBuilder.AppendLine($$"""
                            new IndicatorParamConfig
                            {
                                ParamName = "{{param.Name}}"
                               ,DisplayName = "{{param.DisplayName}}"
                               ,DataType = "{{param.DataType}}"
                               ,DefaultValue = {{defaultValueStr}}
                               ,Minimum = {{minValueStr}}
                               ,Maximum = {{maxValueStr}}
        """);

        // Add enum values if present
        if (param.EnumValues != null && param.EnumValues.Count > 0)
        {
            sourceBuilder.AppendLine("""
                               ,EnumValues = new Dictionary<int, string>
                                {
            """);

            foreach (KeyValuePair<int, string> kvp in param.EnumValues)
            {
                sourceBuilder.AppendLine($"                            [{kvp.Key}] = \"{kvp.Value}\",");
            }

            sourceBuilder.AppendLine("                        }");
        }

        sourceBuilder.AppendLine("                    },");
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
        string Uiid,
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
        double? DefaultValue,
        double? MinValue,
        double? MaxValue,
        string? EnumType = null,
        Dictionary<int, string>? EnumValues = null);


    #region Diagnostic errors

    private static void ReportIND901_DuplicateListings(
        SourceProductionContext context,
        List<string> duplicateUIIDs) => context.ReportDiagnostic(
            diagnostic: Diagnostic.Create(
                descriptor: new DiagnosticDescriptor(
                    id: "IND901",
                    title: "Duplicate UIIDs detected",
                    messageFormat: "The following UIIDs are used more than once: {0}",
                    category: "Catalog",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location: Location.None,
                messageArgs: string.Join(", ", duplicateUIIDs)));

    private static void ReportIND902_InvalidDefaultValue(
        SourceProductionContext context,
        double? defaultValue,
        double? minValue,
        double? maxValue) => context.ReportDiagnostic(
            diagnostic: Diagnostic.Create(
                descriptor: new DiagnosticDescriptor(
                    id: "IND902",
                    title: "Default value must be between min/max value range.",
                    messageFormat: "ParamNum values [{0}, {1}, {2}] are invalid.  Default is out of min/max range.",
                    category: "Catalog",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                location: Location.None,
                messageArgs: [defaultValue, minValue, maxValue]));

    #endregion
}
