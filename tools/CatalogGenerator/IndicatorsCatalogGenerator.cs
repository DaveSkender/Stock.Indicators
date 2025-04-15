using Microsoft.CodeAnalysis.Text;

namespace Stock.Indicators.Generator;

[Generator]
public class IndicatorsCatalogGenerator : ISourceGenerator
{
    private static readonly HashSet<string> IgnoredParameterNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "source",
        "chainProvider",
        "quotes"
    };

    private static string DetermineCategory(string indicatorId, string type)
    {
        // For test/generated indicators (those with Series, StreamHub, or Buffer attributes)
        // use the Generated category
        if (!string.IsNullOrEmpty(type))
        {
            return "Generated";
        }

        // For standard hardcoded indicators, use the specific category
        return indicatorId switch {
            var id when id.Contains("MA", StringComparison.OrdinalIgnoreCase) ||
                       id is "SMA" or "EMA" or "HMA" or "WMA" or "TEMA" or "DEMA" => "moving-average",
            var id when id is "ADX" or "AROON" => "price-trend",
            var id when id is "ADL" or "CMF" or "MFI" => "volume-based",
            var id when id.EndsWith("OSC", StringComparison.OrdinalIgnoreCase) ||
                       id is "RSI" or "MACD" or "CCI" => "oscillator",
            var id when id.Contains("PATTERN", StringComparison.OrdinalIgnoreCase) ||
                       id.Contains("CANDLESTICK", StringComparison.OrdinalIgnoreCase) => "pattern",
            var id when id.EndsWith("BAND", StringComparison.OrdinalIgnoreCase) ||
                       id.Contains("CHANNEL", StringComparison.OrdinalIgnoreCase) ||
                       id is "BB" or "KC" or "DONCHIAN" => "price-channel",
            _ => "Generated" // Default to Generated for any unknown types
        };
    }

    private static string DetermineChartType(string indicatorId) =>
        indicatorId switch {
            var id when id.EndsWith("BAND", StringComparison.OrdinalIgnoreCase) ||
                       id.Contains("CHANNEL", StringComparison.OrdinalIgnoreCase) ||
                       id is "BB" or "KC" or "DONCHIAN" => "overlay",
            var id when id.Contains("OSC", StringComparison.OrdinalIgnoreCase) ||
                       id is "RSI" or "MACD" or "CCI" or "ADX" or "AROON" or "MFI" => "oscillator",
            var id when id.Contains("MA", StringComparison.OrdinalIgnoreCase) ||
                       id is "SMA" or "EMA" or "HMA" or "WMA" or "TEMA" or "DEMA" => "overlay",
            _ => "indicator"
        };

    public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        Compilation compilation = context.Compilation;

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
        foreach (MethodDeclarationSyntax methodSyntax in receiver.CandidateMethodsWithSeriesAttribute)
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
                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Series", methodSymbol.ContainingType.Name, methodSymbol.Name, parameters));
                }
            }
        }

        // Process methods with StreamHubAttribute
        foreach (MethodDeclarationSyntax methodSyntax in receiver.CandidateMethodsWithStreamHubAttribute)
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
                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Stream", methodSymbol.ContainingType.Name, methodSymbol.Name, parameters));
                }
            }
        }

        // Process constructors with BufferAttribute
        foreach (ConstructorDeclarationSyntax constructorSyntax in receiver.CandidateConstructorsWithBufferAttribute)
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
                    List<ParameterInfo> parameters = GetMethodParameters(constructorSymbol, paramAttributeSymbol);

                    indicators.Add(new(id, name, "Buffer", constructorSymbol.ContainingType.Name, "Constructor", parameters));
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
        sourceBuilder.AppendLine("public static class GeneratedIndicatorCatalog");
        sourceBuilder.AppendLine("{");
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Gets the complete list of indicators");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    /// <returns>A list of indicator information</returns>");
        sourceBuilder.AppendLine("    public static IReadOnlyList<IndicatorListing> GetIndicators()");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        return new List<IndicatorListing>");
        sourceBuilder.AppendLine("        {");

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
            sourceBuilder.AppendLine($"                Category = \"{DetermineCategory(indicator.Id, indicator.Type)}\",");
            sourceBuilder.AppendLine($"                ChartType = \"{DetermineChartType(indicator.Id)}\",");
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
        sourceBuilder.AppendLine("    }");
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
        List<ParameterInfo> Parameters);

    private sealed record ParameterInfo(
        string Name,
        string DisplayName,
        double MinValue,
        double MaxValue,
        double DefaultValue);

    private sealed class SyntaxReceiver : ISyntaxReceiver
    {
        public List<MethodDeclarationSyntax> CandidateMethodsWithSeriesAttribute { get; } = [];
        public List<MethodDeclarationSyntax> CandidateMethodsWithStreamHubAttribute { get; } = [];
        public List<ConstructorDeclarationSyntax> CandidateConstructorsWithBufferAttribute { get; } = [];

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Look for methods with attributes
            if (syntaxNode is MethodDeclarationSyntax methodDeclaration &&
                methodDeclaration.AttributeLists.Count > 0)
            {
                foreach (AttributeListSyntax attributeList in methodDeclaration.AttributeLists)
                {
                    foreach (AttributeSyntax attribute in attributeList.Attributes)
                    {
                        string attributeName = attribute.Name.ToString();
                        if (attributeName is "Series" or "SeriesAttribute")
                        {
                            CandidateMethodsWithSeriesAttribute.Add(methodDeclaration);
                        }
                        else if (attributeName is "StreamHub" or "StreamHubAttribute")
                        {
                            CandidateMethodsWithStreamHubAttribute.Add(methodDeclaration);
                        }
                    }
                }
            }

            // Look for constructors with attributes
            if (syntaxNode is ConstructorDeclarationSyntax constructorDeclaration &&
                constructorDeclaration.AttributeLists.Count > 0)
            {
                foreach (AttributeListSyntax attributeList in constructorDeclaration.AttributeLists)
                {
                    foreach (AttributeSyntax attribute in attributeList.Attributes)
                    {
                        string attributeName = attribute.Name.ToString();
                        if (attributeName is "Buffer" or "BufferAttribute")
                        {
                            CandidateConstructorsWithBufferAttribute.Add(constructorDeclaration);
                        }
                    }
                }
            }
        }
    }
}

