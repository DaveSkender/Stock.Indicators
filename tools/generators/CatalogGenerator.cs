using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Stock.Indicators.Generator;

[Generator]
public class CatalogGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the syntax providers
        IncrementalValuesProvider<MethodDeclarationSyntax> methodsWithSeriesAttribute
           = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForSeriesAttribute(s),
                transform: static (ctx, _) => GetMethodWithSeriesAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<MethodDeclarationSyntax> methodsWithStreamAttribute
           = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForStreamAttribute(s),
                transform: static (ctx, _) => GetMethodWithStreamAttribute(ctx))
            .Where(static m => m is not null)!;

        IncrementalValuesProvider<ConstructorDeclarationSyntax> constructorsWithBufferAttribute
           = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsCandidateForBufferAttribute(s),
                transform: static (ctx, _) => GetConstructorWithBufferAttribute(ctx))
            .Where(static c => c is not null)!;

        // Combine all providers and compilation
        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> methodsWithSeriesAttributeProvider
            = methodsWithSeriesAttribute.Collect();

        IncrementalValueProvider<ImmutableArray<MethodDeclarationSyntax>> methodsWithStreamAttributeProvider
            = methodsWithStreamAttribute.Collect();

        IncrementalValueProvider<ImmutableArray<ConstructorDeclarationSyntax>> constructorsWithBufferAttributeProvider
            = constructorsWithBufferAttribute.Collect();

        // Create a combined provider with the compilation and all three collections
        IncrementalValueProvider<(
             ((Compilation Left, ImmutableArray<MethodDeclarationSyntax> Right) Left,
               ImmutableArray<MethodDeclarationSyntax> Right
             ) Left,
             ImmutableArray<ConstructorDeclarationSyntax> Right
            )> combined = context.CompilationProvider
              .Combine(methodsWithSeriesAttributeProvider)
              .Combine(methodsWithStreamAttributeProvider)
              .Combine(constructorsWithBufferAttributeProvider);

        // Register the source output generation
        context.RegisterSourceOutput(combined, (spc, tuple) => {
            Compilation compilation = tuple.Left.Left.Left;
            ImmutableArray<MethodDeclarationSyntax> seriesAttributes = tuple.Left.Left.Right;
            ImmutableArray<MethodDeclarationSyntax> StreamAttributes = tuple.Left.Right;
            ImmutableArray<ConstructorDeclarationSyntax> bufferAttributes = tuple.Right;

            Execute(spc, compilation, seriesAttributes, StreamAttributes, bufferAttributes);
        });
    }

    private static bool IsCandidateForSeriesAttribute(SyntaxNode node)
        => node is MethodDeclarationSyntax method &&
            method.AttributeLists.Count > 0 &&
            method.AttributeLists.Any(al => al.Attributes.Any(a =>
                a.Name.ToString() is "Series" or "SeriesAttribute"));

    private static bool IsCandidateForStreamAttribute(SyntaxNode node)
        => node is MethodDeclarationSyntax method &&
            method.AttributeLists.Count > 0 &&
            method.AttributeLists.Any(al => al.Attributes.Any(a =>
                a.Name.ToString() is "StreamHub" or "StreamAttribute"));

    private static bool IsCandidateForBufferAttribute(SyntaxNode node)
        => node is ConstructorDeclarationSyntax constructor &&
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

    private static MethodDeclarationSyntax? GetMethodWithStreamAttribute(GeneratorSyntaxContext context)
    {
        MethodDeclarationSyntax methodDeclaration = (MethodDeclarationSyntax)context.Node;
        foreach (AttributeListSyntax attributeList in methodDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                string attributeName = attribute.Name.ToString();
                if (attributeName is "StreamHub" or "StreamAttribute")
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
        ImmutableArray<MethodDeclarationSyntax> methodsWithStreamAttribute,
        ImmutableArray<ConstructorDeclarationSyntax> constructorsWithBufferAttribute)
    {
        if (methodsWithSeriesAttribute.IsDefaultOrEmpty &&
            methodsWithStreamAttribute.IsDefaultOrEmpty &&
            constructorsWithBufferAttribute.IsDefaultOrEmpty)
        {
            return;
        }

        INamedTypeSymbol? seriesAttributeSymbol
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.SeriesAttribute")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.SeriesAttribute");

        INamedTypeSymbol? streamAttributeSymbol
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.StreamAttribute")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.StreamAttribute");

        INamedTypeSymbol? bufferAttributeSymbol
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.BufferAttribute")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.BufferAttribute");

        INamedTypeSymbol? paramAttributeSymbol
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.ParamAttribute")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.ParamAttribute");

        INamedTypeSymbol? categoryEnum
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.Category")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.Category");

        INamedTypeSymbol? chartTypeEnum
            = compilation.GetTypeByMetadataName("Stock.Indicators.Generator.Test.ChartType")
           ?? compilation.GetTypeByMetadataName("Skender.Stock.Indicators.ChartType");

        if (seriesAttributeSymbol is null
         || streamAttributeSymbol is null
         || bufferAttributeSymbol is null
         || paramAttributeSymbol is null
         || categoryEnum is null
         || chartTypeEnum is null)
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

                    // Convert category from enum to string
                    object? categoryValue = attributeData.ConstructorArguments[2].Value;
                    string category = categoryValue != null ?
                        GetEnumFieldName(categoryEnum, Convert.ToInt32(categoryValue)) : string.Empty;

                    // Convert chartType from enum to string
                    object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                    string chartType = chartTypeValue != null ?
                        GetEnumFieldName(chartTypeEnum, Convert.ToInt32(chartTypeValue)) : string.Empty;

                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(
                        Id: id,
                        Name: name,
                        Type: "Series",
                        ContainingType: methodSymbol.ContainingType.Name,
                        MemberName: methodSymbol.Name,
                        Category: category,
                        ChartType: chartType,
                        Parameters: parameters));
                }
            }
        }

        // Process methods with StreamAttribute
        foreach (MethodDeclarationSyntax methodSyntax in methodsWithStreamAttribute)
        {
            SemanticModel semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(methodSyntax) is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            foreach (AttributeData attributeData in methodSymbol.GetAttributes())
            {
                INamedTypeSymbol? attributeClass = attributeData.AttributeClass;
                if (attributeClass?.Equals(streamAttributeSymbol, SymbolEqualityComparer.Default) == true)
                {
                    string id = attributeData.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(id) || !processedIds.Add(id))
                    {
                        continue;
                    }

                    string name = attributeData.ConstructorArguments[1].Value?.ToString() ?? string.Empty;

                    // Convert category from enum to string
                    object? categoryValue = attributeData.ConstructorArguments[2].Value;
                    string category = categoryValue != null ?
                        GetEnumFieldName(categoryEnum, Convert.ToInt32(categoryValue)) : string.Empty;

                    // Convert chartType from enum to string
                    object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                    string chartType = chartTypeValue != null ?
                        GetEnumFieldName(chartTypeEnum, Convert.ToInt32(chartTypeValue)) : string.Empty;

                    List<ParameterInfo> parameters = GetMethodParameters(methodSymbol, paramAttributeSymbol);

                    indicators.Add(new(
                        Id: id,
                        Name: name,
                        Type: "Stream",
                        ContainingType: methodSymbol.ContainingType.Name,
                        MemberName: methodSymbol.Name,
                        Category: category,
                        ChartType: chartType,
                        Parameters: parameters));
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

                    // Convert category from enum to string
                    object? categoryValue = attributeData.ConstructorArguments[2].Value;
                    string category = categoryValue != null ?
                        GetEnumFieldName(categoryEnum, Convert.ToInt32(categoryValue)) : string.Empty;

                    // Convert chartType from enum to string
                    object? chartTypeValue = attributeData.ConstructorArguments[3].Value;
                    string chartType = chartTypeValue != null ?
                        GetEnumFieldName(chartTypeEnum, Convert.ToInt32(chartTypeValue)) : string.Empty;

                    List<ParameterInfo> parameters = GetMethodParameters(constructorSymbol, paramAttributeSymbol);

                    indicators.Add(new(
                        Id: id,
                        Name: name,
                        Type: "Buffer",
                        ContainingType: constructorSymbol.ContainingType.Name,
                        MemberName: "Constructor",
                        Category: category,
                        ChartType: chartType,
                        Parameters: parameters));
                }
            }
        }

        // Generate catalog class
        StringBuilder sourceBuilder = new();
        sourceBuilder.AppendLine("// <auto-generated/>");
        sourceBuilder.AppendLine("using System.Collections.Generic;");
        sourceBuilder.AppendLine("using System.Linq;");
        sourceBuilder.AppendLine("using System.CodeDom.Compiler;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("namespace Skender.Stock.Indicators;");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("/// <summary>");
        sourceBuilder.AppendLine("/// Auto-generated catalog of all indicators in the library");
        sourceBuilder.AppendLine("/// </summary>");
        sourceBuilder.AppendLine("[GeneratedCode(\"Stock.Indicators.Generator\", \"1.0.0\")]");
        sourceBuilder.AppendLine("public static partial class GeneratedCatalog");
        sourceBuilder.AppendLine("{");

        // Generate the AllIndicators property implementation
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Gets all indicators (auto-generated and test indicators)");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    private static partial IReadOnlyList<IndicatorListing> AllIndicators");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        get");
        sourceBuilder.AppendLine("        {");
        sourceBuilder.AppendLine("            var autoGenIndicators = new List<IndicatorListing>");
        sourceBuilder.AppendLine("            {");

        // Add all found indicators to the collection
        foreach (IndicatorInfo? indicator in indicators.OrderBy(i => i.Name))
        {
            sourceBuilder.AppendLine("                new IndicatorListing");
            sourceBuilder.AppendLine("                {");

            // Append the indicator style to the name if it's available
            string displayName = indicator.Name;
            if (!string.IsNullOrEmpty(indicator.Type) && !indicator.Name.Contains(indicator.Type))
            {
                displayName = $"{indicator.Name} ({indicator.Type})";
            }

            sourceBuilder.AppendLine($"                    Name = \"{displayName}\",");
            sourceBuilder.AppendLine($"                    Uiid = \"{indicator.Id}\",");
            sourceBuilder.AppendLine($"                    Category = Category.{indicator.Category},");
            sourceBuilder.AppendLine($"                    ChartType = ChartType.{indicator.ChartType},");
            sourceBuilder.AppendLine($"                    Order = Order.Front,");
            sourceBuilder.AppendLine($"                    ChartConfig = null,");

            if (indicator.Parameters.Count > 0)
            {
                sourceBuilder.AppendLine("                    Parameters = new List<IndicatorParamConfig>");
                sourceBuilder.AppendLine("                    {");

                foreach (ParameterInfo param in indicator.Parameters)
                {
                    sourceBuilder.AppendLine("                        new IndicatorParamConfig");
                    sourceBuilder.AppendLine("                        {");
                    sourceBuilder.AppendLine($"                            ParamName = \"{param.Name}\",");
                    sourceBuilder.AppendLine($"                            DisplayName = \"{param.DisplayName}\",");
                    sourceBuilder.AppendLine($"                            DataType = \"{param.DataType}\",");

                    // Format numeric values without 'd' suffix
                    string minValueStr = FormatNumericValue(param.MinValue);
                    string maxValueStr = FormatNumericValue(param.MaxValue);
                    string defaultValueStr = FormatNumericValue(param.DefaultValue);

                    sourceBuilder.AppendLine($"                            Minimum = {minValueStr},");
                    sourceBuilder.AppendLine($"                            Maximum = {maxValueStr},");
                    sourceBuilder.AppendLine($"                            DefaultValue = {defaultValueStr}");
                    sourceBuilder.AppendLine("                        },");
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
                tooltipTemplate += "(" + string.Join(",", Enumerable.Range(1, indicator.Parameters.Count).Select(i => $"[P{i}]")) + ")";
            }

            sourceBuilder.AppendLine("                    Results = new List<IndicatorResultConfig>");
            sourceBuilder.AppendLine("                    {");
            sourceBuilder.AppendLine("                        new IndicatorResultConfig");
            sourceBuilder.AppendLine("                        {");
            sourceBuilder.AppendLine($"                            DataName = \"{indicator.Id.ToLowerInvariant()}\",");
            sourceBuilder.AppendLine($"                            DisplayName = \"{indicator.Name}\",");
            sourceBuilder.AppendLine($"                            TooltipTemplate = \"{tooltipTemplate}\",");
            sourceBuilder.AppendLine("                            DataType = \"number\",");
            sourceBuilder.AppendLine("                            DefaultColor = ChartColors.StandardBlue,");
            sourceBuilder.AppendLine("                            LineType = \"solid\",");
            sourceBuilder.AppendLine("                            LineWidth = 2,");
            sourceBuilder.AppendLine("                            Stack = null,");
            sourceBuilder.AppendLine("                            Fill = null");
            sourceBuilder.AppendLine("                        }");
            sourceBuilder.AppendLine("                    }");

            sourceBuilder.AppendLine("                },");
        }

        sourceBuilder.AppendLine("            };");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("            // Combine real indicators with test indicators");
        sourceBuilder.AppendLine("            var combinedList = new List<IndicatorListing>(autoGenIndicators);");
        sourceBuilder.AppendLine("            combinedList.AddRange(TestIndicators);");
        sourceBuilder.AppendLine("            return combinedList;");
        sourceBuilder.AppendLine("        }");
        sourceBuilder.AppendLine("    }");
        sourceBuilder.AppendLine("}");

        context.AddSource("GeneratedCatalog.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
    }

    /// <summary>
    /// Retrieves the name of an enum field from its integer value.
    /// </summary>
    /// <param name="enumType">The enum type symbol.</param>
    /// <param name="value">The integer value of the enum.</param>
    /// <returns>The name of the enum field, or an empty string if not found.</returns>
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

            // Validate default is within min/max bounds
            if (defaultValue < minValue)
            {
                defaultValue = minValue;
            }
            else if (defaultValue > maxValue)
            {
                defaultValue = maxValue;
            }

            parameters.Add(new(parameter.Name, displayName, dataType, minValue, maxValue, defaultValue));
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
}
