using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Indicators.Catalog.Generator;

/// <summary>
/// Source generator for catalog information about indicators from attributes.
/// </summary>
[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    // Get version dynamically - uses compile-time constant in CI or timestamp locally
    private static string GetGeneratorVersion()
    {
#if VERSIONED_BUILD
        // This value will be replaced during CI build
        return "#{PACKAGE_VERSION}#";
#else
        // For local builds, use a timestamp-based version
        return DateTime.Now.ToString("yyyy.MM.dd-HH:mm:ss.fff");
#endif
    }

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

        // Generate catalog class
        string sourceCode = GenerateCatalogClass(indicators);
        context.AddSource("GeneratedCatalog.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static Dictionary<string, INamedTypeSymbol?> GetRequiredSymbols(Compilation compilation)
    {
        const string srcNamespace = "Skender.Stock.Indicators";

        return new() {
            ["SeriesAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.SeriesAttribute"),
            ["StreamAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.StreamAttribute"),
            ["BufferAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.BufferAttribute"),
            ["ParamAttribute"] = compilation.GetTypeByMetadataName($"{srcNamespace}.ParamAttribute"),
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

                // Process parameters
                List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, symbols["ParamAttribute"]);

                // Create indicator info
                indicators.Add(new IndicatorInfo(
                    Id: id,
                    Name: name,
                    Type: attributeType,
                    ContainingType: methodSymbol.ContainingType.Name,
                    MemberName: methodSymbol.Name == ".ctor" ? "Constructor" : methodSymbol.Name,
                    Category: category,
                    ChartType: chartType,
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
        INamedTypeSymbol? paramAttributeSymbol)
    {
        List<ParameterInfo> parameters = [];

        // If paramAttributeSymbol is null, we can't identify parameters
        if (paramAttributeSymbol is null)
        {
            return parameters;
        }

        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            // Only include parameters that have the ParamAttribute
            AttributeData? paramAttribute = parameter.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Equals(paramAttributeSymbol, SymbolEqualityComparer.Default) == true);

            if (paramAttribute == null)
            {
                continue; // Skip parameters without ParamAttribute
            }

            // Extract information from the attribute
            string displayName = paramAttribute.ConstructorArguments[0].Value?.ToString() ?? parameter.Name;
            double minValue = Convert.ToDouble(paramAttribute.ConstructorArguments[1].Value ?? 0);
            double maxValue = Convert.ToDouble(paramAttribute.ConstructorArguments[2].Value ?? double.MaxValue);
            double defaultValue = Convert.ToDouble(paramAttribute.ConstructorArguments[3].Value ?? 0);

            // Determine the parameter type based on the parameter's type symbol
            string dataType = DetermineDataType(parameter.Type);

            // Ensure default is within min/max bounds
            defaultValue = Math.Min(Math.Max(defaultValue, minValue), maxValue);

            parameters.Add(new ParameterInfo(
                parameter.Name, displayName, dataType, minValue, maxValue, defaultValue));
        }

        return parameters;
    }

    /// <summary>
    /// Determines the appropriate JavaScript/TypeScript data type based on the parameter type.
    /// </summary>
    private static string DetermineDataType(ITypeSymbol typeSymbol)
    {
        // Check the parameter's declared type
        string typeName = typeSymbol.ToString().ToLowerInvariant();

        if (typeName is "int" or "int32" or "int16" or
            "byte" or "sbyte" or "uint" or
            "uint32" or "uint16" or "short" or
            "ushort" or "long" or "ulong")
        {
            return "int";
        }

        if (typeName is "bool" or "boolean")
        {
            return "boolean";
        }

        if (typeName == "string")
        {
            return "string";
        }

        // For decimal/double/float types and any other numeric types
        return "number";
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
        sourceBuilder.AppendLine("public static partial class GeneratedCatalog");
        sourceBuilder.AppendLine("{");

        // Generate the AllIndicators property implementation
        sourceBuilder.AppendLine("""
            /// <summary>
            /// Gets all indicators (auto-generated and test indicators)
            /// </summary>
            private static partial IReadOnlyList<IndicatorListing> AllIndicators
            {
                get
                {
                    var autoGenIndicators = new List<IndicatorListing>
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

                    // Combine real indicators with test indicators
                    var combinedList = new List<IndicatorListing>(autoGenIndicators);
                    combinedList.AddRange(TestIndicators);
                    return combinedList;
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

        sourceBuilder.AppendLine($$"""
                new IndicatorListing
                {
                    Name = "{{displayName}}",
                    Uiid = "{{indicator.Id}}",
                    Category = Category.{{indicator.Category}},
                    ChartType = ChartType.{{indicator.ChartType}},
                    Order = Order.Front,
                    ChartConfig = null,
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

        // Build tooltip template based on parameters
        string tooltipTemplate = $"{indicator.Id}";
        if (indicator.Parameters.Count > 0)
        {
            tooltipTemplate += "("
                + string.Join(",",
                    Enumerable
                        .Range(1, indicator.Parameters.Count)
                        .Select(i => $"[P{i}]")
                    ) + ")";
        }

        // Add result configs
        AppendResultConfig(sourceBuilder, indicator.Name, indicator.Id, tooltipTemplate);

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
                            Minimum = {{minValueStr}},
                            Maximum = {{maxValueStr}},
                            DefaultValue = {{defaultValueStr}}
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
        return value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }

    private sealed record IndicatorInfo(
        string Id,
        string Name,
        string Type,
        string ContainingType,
        string MemberName,
        string Category,
        string ChartType,
        List<ParameterInfo> Parameters);

    private sealed record ParameterInfo(
        string Name,
        string DisplayName,
        string DataType,
        double MinValue,
        double MaxValue,
        double DefaultValue);
}
