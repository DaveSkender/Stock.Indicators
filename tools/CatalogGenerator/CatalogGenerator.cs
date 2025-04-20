using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Stock.Indicators.Generator;

[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    private static readonly HashSet<string> IgnoredParameterNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "source",
        "chainProvider",
        "quotes"
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the syntax providers
        IncrementalValuesProvider<MethodDeclarationSyntax> methodsWithSeriesAttribute = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForSeriesAttribute(s),
                transform: static (ctx, _) => GetMethodWithSeriesAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<MethodDeclarationSyntax> methodsWithStreamHubAttribute = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForStreamHubAttribute(s),
                transform: static (ctx, _) => GetMethodWithStreamHubAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<ConstructorDeclarationSyntax> constructorsWithBufferAttribute = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForBufferAttribute(s),
                transform: static (ctx, _) => GetConstructorWithBufferAttribute(ctx))
            .Where(static c => c is not null)!;

        // Combine all providers and compilation
        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> methodsWithSeriesAttributeProvider = methodsWithSeriesAttribute.Collect();
        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> methodsWithStreamHubAttributeProvider = methodsWithStreamHubAttribute.Collect();
        IncrementalValueProvider<ImmutableArray<ConstructorDeclarationSyntax>> constructorsWithBufferAttributeProvider = constructorsWithBufferAttribute.Collect();

        // Create a combined provider with the compilation and all three collections
        IncrementalValueProvider<(((Compilation Left, ImmutableArray<MethodDeclarationSyntax> Right) Left, ImmutableArray<MethodDeclarationSyntax> Right) Left, ImmutableArray<ConstructorDeclarationSyntax> Right)> combined = context.CompilationProvider.Combine(
            methodsWithSeriesAttributeProvider).Combine(
                methodsWithStreamHubAttributeProvider).Combine(
                    constructorsWithBufferAttributeProvider);

        // Register the source output generation
        context.RegisterSourceOutput(combined, (spc, tuple) => {
            Compilation compilation = tuple.Left.Left.Left;
            ImmutableArray<MethodDeclarationSyntax> seriesAttributes = tuple.Left.Left.Right;
            ImmutableArray<MethodDeclarationSyntax> streamHubAttributes = tuple.Left.Right;
            ImmutableArray<ConstructorDeclarationSyntax> bufferAttributes = tuple.Right;

            Execute(spc, compilation, seriesAttributes, streamHubAttributes, bufferAttributes);
        });
    }

    private static bool IsCandidateForSeriesAttribute(SyntaxNode node) => node is MethodDeclarationSyntax method &&
            method.AttributeLists.Count > 0 &&
            method.AttributeLists.Any(al => al.Attributes.Any(a =>
                a.Name.ToString() is "Series" or "SeriesAttribute"));

    private static bool IsCandidateForStreamHubAttribute(SyntaxNode node) => node is MethodDeclarationSyntax method &&
            method.AttributeLists.Count > 0 &&
            method.AttributeLists.Any(al => al.Attributes.Any(a =>
                a.Name.ToString() is "StreamHub" or "StreamHubAttribute"));

    private static bool IsCandidateForBufferAttribute(SyntaxNode node) => node is ConstructorDeclarationSyntax constructor &&
            constructor.AttributeLists.Count > 0 &&
            constructor.AttributeLists.Any(al => al.Attributes.Any(a =>
                a.Name.ToString() is "Buffer" or "BufferAttribute"));

    private static MethodDeclarationSyntax? GetMethodWithSeriesAttribute(GeneratorSyntaxContext context)
    {
        MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)context.Node;
        foreach (AttributeListSyntax attributeList in methodDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                string attributeName = attribute.Name.ToString();
                if (attributeName is "Series" or "SeriesAttribute")
                {
                    return methodDeclaration;
                }
            }
        }

        return null;
    }

    private static MethodDeclarationSyntax? GetMethodWithStreamHubAttribute(GeneratorSyntaxContext context)
    {
        MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)context.Node;
        foreach (AttributeListSyntax attributeList in methodDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                string attributeName = attribute.Name.ToString();
                if (attributeName is "StreamHub" or "StreamHubAttribute")
                {
                    return methodDeclaration;
                }
            }
        }

        return null;
    }

    private static ConstructorDeclarationSyntax? GetConstructorWithBufferAttribute(GeneratorSyntaxContext context)
    {
        ConstructorDeclarationSyntax constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;
        foreach (AttributeListSyntax attributeList in constructorDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                string attributeName = attribute.Name.ToString();
                if (attributeName is "Buffer" or "BufferAttribute")
                {
                    return constructorDeclaration;
                }
            }
        }

        return null;
    }

    private static void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<MethodDeclarationSyntax> methodsWithSeriesAttribute,
        ImmutableArray<MethodDeclarationSyntax> methodsWithStreamHubAttribute,
        ImmutableArray<ConstructorDeclarationSyntax> constructorsWithBufferAttribute)
    {
        if (methodsWithSeriesAttribute.IsDefaultOrEmpty &&
            methodsWithStreamHubAttribute.IsDefaultOrEmpty &&
            constructorsWithBufferAttribute.IsDefaultOrEmpty)
        {
            return;
        }

        INamedTypeSymbol? seriesAttributeSymbol = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.SeriesAttribute") ??
                                compilation.GetTypeByMetadataName("Skender.Stock.Indicators.SeriesAttribute");
        INamedTypeSymbol? streamHubAttributeSymbol = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.StreamHubAttribute") ??
                                   compilation.GetTypeByMetadataName("Skender.Stock.Indicators.StreamHubAttribute");
        INamedTypeSymbol? bufferAttributeSymbol = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.BufferAttribute") ??
                                 compilation.GetTypeByMetadataName("Skender.Stock.Indicators.BufferAttribute");
        INamedTypeSymbol? paramAttributeSymbol = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.ParamAttribute") ??
                                compilation.GetTypeByMetadataName("Skender.Stock.Indicators.ParamAttribute");

        if (seriesAttributeSymbol is null || streamHubAttributeSymbol is null ||
            bufferAttributeSymbol is null || paramAttributeSymbol is null)
        {
            return;
        }

        List<IndicatorInfo> indicators = [];
        HashSet<string> processedIds = [];

        // Process methods with SeriesAttribute
        foreach (MethodDeclarationSyntax methodSyntax in methodsWithSeriesAttribute)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in methodSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                if (attributeClass?.Equals(seriesAttributeSymbol, SymbolEqualityComparer.Default) == true)
                {
                    string id = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(id) || !processedIds.Add(id))
                    {
                        continue;
                    }

                    string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
                    string category = attributeData.ConstructorArguments[2].Value?.ToString() ?? string.Empty;
                    string chartType = attributeData.ConstructorArguments[3].Value?.ToString() ?? string.Empty;
                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Series", methodSymbol.ContainingType.Name, methodSymbol.Name, category, chartType, parameters));
                }
            }
        }

        // Process methods with StreamHubAttribute
        foreach (MethodDeclarationSyntax methodSyntax in methodsWithStreamHubAttribute)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in methodSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                if (attributeClass?.Equals(streamHubAttributeSymbol, SymbolEqualityComparer.Default) == true)
                {
                    string id = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(id) || !processedIds.Add(id))
                    {
                        continue;
                    }

                    string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
                    string category = attributeData.ConstructorArguments[2].Value?.ToString() ?? string.Empty;
                    string chartType = attributeData.ConstructorArguments[3].Value?.ToString() ?? string.Empty;
                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Stream", methodSymbol.ContainingType.Name, methodSymbol.Name, category, chartType, parameters));
                }
            }
        }

        // Process constructors with BufferAttribute
        foreach (ConstructorDeclarationSyntax constructorSyntax in constructorsWithBufferAttribute)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(constructorSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(constructorSyntax) is not IMethodSymbol constructorSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in constructorSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                if (attributeClass?.Equals(bufferAttributeSymbol, SymbolEqualityComparer.Default) == true)
                {
                    string id = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(id) || !processedIds.Add(id))
                    {
                        continue;
                    }

                    string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
                    string category = attributeData.ConstructorArguments[2].Value?.ToString() ?? string.Empty;
                    string chartType = attributeData.ConstructorArguments[3].Value?.ToString() ?? string.Empty;
                    List<ParameterInfo> parameters = GetMethodParameters(constructorSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Buffer", constructorSymbol.ContainingType.Name, "Constructor", category, chartType, parameters));
                }
            }
        }

        // Generate catalog class
        StringBuilder sourceBuilder = new();
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("using System.Collections.Generic;");
        sourceBuilder.AppendLine("using System.Linq;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("namespace Skender.Stock.Indicators;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("/// <summary>");
        sourceBuilder.AppendLine("/// Auto-generated catalog of all indicators in the library");
        sourceBuilder.AppendLine("/// </summary>");
        sourceBuilder.AppendLine("public static partial class GeneratedIndicatorCatalog");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Gets the auto-generated list of indicators");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    public static IReadOnlyList<IndicatorListing> AutoGeneratedIndicators => new List<IndicatorListing>");
        sourceBuilder.AppendLine("    {");

        foreach (IndicatorInfo? indicator in indicators.OrderBy(i => i.Name))
        {
            sourceBuilder.AppendLine("            new IndicatorListing");
            sourceBuilder.AppendLine("            {");

            // Append the indicator style to the name if it's available
            string displayName = indicator.Name;
            if (!string.IsNullOrEmpty(indicator.Type) && !indicator.Name.Contains(indicator.Type))
            {
                displayName = $"{indicator.Name} ({indicator.Type})";
            }

            sourceBuilder.AppendLine($"                Name = \"{displayName}\",");
            sourceBuilder.AppendLine($"                Uiid = \"{indicator.Id}\",");
            sourceBuilder.AppendLine($"                Category = \"{indicator.Category}\",");
            sourceBuilder.AppendLine($"                ChartType = \"{indicator.ChartType}\",");
            sourceBuilder.AppendLine($"                Order = Order.Front,");
            sourceBuilder.AppendLine($"                ChartConfig = null,");

            if (indicator.Parameters.Count > 0)
            {
                sourceBuilder.AppendLine("                Parameters = new List<IndicatorParamConfig>");
                sourceBuilder.AppendLine("                {");

                foreach (ParameterInfo param in indicator.Parameters)
                {
                    sourceBuilder.AppendLine("                    new IndicatorParamConfig");
                    sourceBuilder.AppendLine("                    {");
                    sourceBuilder.AppendLine($"                        ParamName = \"{param.Name}\",");
                    sourceBuilder.AppendLine($"                        DisplayName = \"{param.DisplayName}\",");
                    sourceBuilder.AppendLine($"                        DataType = \"int\",");
                    sourceBuilder.AppendLine($"                        Minimum = {param.MinValue},");
                    sourceBuilder.AppendLine($"                        Maximum = {param.MaxValue},");
                    sourceBuilder.AppendLine($"                        DefaultValue = {param.DefaultValue}");
                    sourceBuilder.AppendLine("                    },");
                }

                sourceBuilder.AppendLine("                },");
            }
            else
            {
                sourceBuilder.AppendLine("                Parameters = new List<IndicatorParamConfig>(),");
            }

            // Build tooltip template based on parameters
            string tooltipTemplate = $"{indicator.Id}";
            if (indicator.Parameters.Count > 0)
            {
                tooltipTemplate += "(" + string.Join(",", Enumerable.Range(1, indicator.Parameters.Count).Select(i => $"[P{i}]")) + ")";
            }

            sourceBuilder.AppendLine("                Results = new List<IndicatorResultConfig>");
            sourceBuilder.AppendLine("                {");
            sourceBuilder.AppendLine("                    new IndicatorResultConfig");
            sourceBuilder.AppendLine("                    {");
            sourceBuilder.AppendLine($"                        DataName = \"{indicator.Id.ToLowerInvariant()}\",");
            sourceBuilder.AppendLine($"                        DisplayName = \"{indicator.Name}\",");
            sourceBuilder.AppendLine($"                        TooltipTemplate = \"{tooltipTemplate}\",");
            sourceBuilder.AppendLine("                        DataType = \"number\",");
            sourceBuilder.AppendLine("                        DefaultColor = ChartColors.StandardBlue,");
            sourceBuilder.AppendLine("                        LineType = \"solid\",");
            sourceBuilder.AppendLine("                        LineWidth = 2,");
            sourceBuilder.AppendLine("                        Stack = null,");
            sourceBuilder.AppendLine("                        Fill = null");
            sourceBuilder.AppendLine("                    }");
            sourceBuilder.AppendLine("                }");

            sourceBuilder.AppendLine("            },");
        }

        sourceBuilder.AppendLine("        };");
        sourceBuilder.AppendLine("}");

        context.AddSource("GeneratedIndicatorCatalog.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    private static List<ParameterInfo> GetMethodParameters(IMethodSymbol methodSymbol, INamedTypeSymbol? paramAttributeSymbol)
    {
        List<ParameterInfo> parameters = [];
        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            // Skip parameters that shouldn't be exposed in the catalog
            if (IgnoredParameterNames.Contains(parameter.Name))
            {
                continue;
            }

            string displayName = parameter.Name;
            double minValue = 0.0;
            double maxValue = double.MaxValue;
            double defaultValue = 0.0;

            if (paramAttributeSymbol is not null)
            {
                foreach (AttributeData attribute in parameter.GetAttributes())
                {
                    if (attribute.AttributeClass?.Equals(paramAttributeSymbol, SymbolEqualityComparer.Default) == true)
                    {
                        displayName = attribute.ConstructorArguments[0].Value?.ToString() ?? parameter.Name;
                        minValue = Convert.ToDouble(attribute.ConstructorArguments[1].Value ?? 0);
                        maxValue = Convert.ToDouble(attribute.ConstructorArguments[2].Value ?? double.MaxValue);
                        defaultValue = Convert.ToDouble(attribute.ConstructorArguments[3].Value ?? 0);
                        break;
                    }
                }
            }

            parameters.Add(new(parameter.Name, displayName, minValue, maxValue, defaultValue));
        }

        return parameters;
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
        double MinValue,
        double MaxValue,
        double DefaultValue);
}
