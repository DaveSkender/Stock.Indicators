// CodeGeneration.cs
// Helper methods for generating code in the CatalogGenerator
using System;
using System.Collections.Generic;
using System.Text;
using Generators.Catalogger.Helpers;
using Generators.Catalogger.Models;

namespace Generators.Catalogger.Helpers;

/// <summary>
/// Helper for generating C# code for indicator catalog registration
/// </summary>
public static class CodeGeneration
{
    /// <summary>
    /// Generates the Listing property implementation for an indicator class.
    /// </summary>
    /// <param name="classInfo">Information about the indicator class</param>
    /// <returns>Generated C# code for the Listing property</returns>
    public static string GenerateListingProperty(IndicatorClassInfo classInfo)
    {
        var sourceBuilder = new StringBuilder();

        // Generate the Listing property
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Gets the catalog listing for this indicator.");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    public static IndicatorListing Listing => GetListing();");
        sourceBuilder.AppendLine();

        return sourceBuilder.ToString();
    }

    /// <summary>
    /// Generates the GetListing method implementation for an indicator class.
    /// </summary>
    /// <param name="classInfo">Information about the indicator class</param>
    /// <returns>Generated C# code for the GetListing method</returns>
    public static string GenerateGetListingMethod(IndicatorClassInfo classInfo)
    {
        var sourceBuilder = new StringBuilder();

        // Generate method header
        sourceBuilder.AppendLine("    /// <summary>");
        sourceBuilder.AppendLine("    /// Gets the catalog listing for this indicator.");
        sourceBuilder.AppendLine("    /// </summary>");
        sourceBuilder.AppendLine("    private static IndicatorListing GetListing()");
        sourceBuilder.AppendLine("    {");
        sourceBuilder.AppendLine("        var builder = new IndicatorListingBuilder()");

        // Add basic attributes
        sourceBuilder.AppendLine($"            .WithId(\"{classInfo.AttributeInfo.Id}\")");
        sourceBuilder.AppendLine($"            .WithName(\"{classInfo.AttributeInfo.Name ?? classInfo.ClassName}\")");

        // Add description if available
        if (!string.IsNullOrEmpty(classInfo.AttributeInfo.Description))
        {
            sourceBuilder.AppendLine($"            .WithDescription(\"{classInfo.AttributeInfo.Description}\")");
        }

        // Add URL if available
        if (!string.IsNullOrEmpty(classInfo.AttributeInfo.Url))
        {
            sourceBuilder.AppendLine($"            .WithUrl(\"{classInfo.AttributeInfo.Url}\")");
        }

        // Add styles if available
        if (classInfo.HasMultipleStyles && classInfo.Styles.Length > 0)
        {
            sourceBuilder.AppendLine($"            .WithStyles(new[] {{ {string.Join(", ", FormatStringArray(classInfo.Styles))} }})");
        }

        sourceBuilder.AppendLine();

        // Add parameters
        foreach (var param in classInfo.Parameters)
        {
            // Build argument lines for this parameter
            var argumentLines = BuildParameterArgumentLines(param);

            // Check if this is an enum type
            bool isEnum = TypeFormatting.IsEnumType(param.Type);

            if (isEnum)
            {
                // Use AddEnumParameter for enum types
                // Note: We need to extract just the type name, not the fully qualified name
                string typeName = GetParameterType(param.Type);
                sourceBuilder.AppendLine($"            builder.AddEnumParameter<{typeName}>(");
            }
            else
            {
                // Use AddParameter for non-enum types
                sourceBuilder.AppendLine($"            builder.AddParameter<{GetParameterType(param.Type)}>(");
            }

            // Add the argument lines
            for (int i = 0; i < argumentLines.Count; i++)
            {
                string comma = (i < argumentLines.Count - 1) ? "," : string.Empty;
                sourceBuilder.AppendLine($"                {argumentLines[i]}{comma}");
            }
            sourceBuilder.AppendLine("            );");
            sourceBuilder.AppendLine();
        }

        // Add result
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("            // Add result based on detected type");
        sourceBuilder.AppendLine("            builder.AddResult(");
        sourceBuilder.AppendLine("                dataName: \"Result\",");
        sourceBuilder.AppendLine("                displayName: \"Result\",");
        sourceBuilder.AppendLine($"                dataType: ResultType.{classInfo.ResultInfo.ResultType},");
        sourceBuilder.AppendLine("                isDefault: true);");
        sourceBuilder.AppendLine();
        sourceBuilder.AppendLine("            return builder.Build();");
        sourceBuilder.AppendLine("        }");

        return sourceBuilder.ToString();
    }

    /// <summary>
    /// Builds the argument lines for a parameter.
    /// </summary>
    private static List<string> BuildParameterArgumentLines(ParameterInfo param)
    {
        var argumentLines = new List<string>();

        // Add arguments in the correct order for AddParameter<T>:
        // parameterName, displayName, description, isRequired, defaultValue, minimum, maximum
        argumentLines.Add($"parameterName: \"{param.Name}\"");
        argumentLines.Add($"displayName: \"{FormatDisplayName(param.Name)}\"");
        argumentLines.Add($"description: null"); // No description available in ParameterInfo
        argumentLines.Add($"isRequired: {(!param.HasExplicitDefaultValue).ToString().ToLowerInvariant()}");
        if (param.HasExplicitDefaultValue)
        {
            bool isEnum = TypeFormatting.IsEnumType(param.Type);
            if (isEnum)
            {
                string typeName = GetParameterType(param.Type);
                string enumValueText = param.ExplicitDefaultValue?.ToString() ?? string.Empty;
                if (enumValueText.Contains("."))
                {
                    int lastDot = enumValueText.LastIndexOf('.');
                    if (lastDot >= 0 && lastDot < enumValueText.Length - 1)
                    {
                        enumValueText = enumValueText.Substring(lastDot + 1);
                    }
                }
                if (!string.IsNullOrEmpty(enumValueText))
                {
                    argumentLines.Add($"defaultValue: {typeName}.{enumValueText}");
                }
                else
                {
                    argumentLines.Add($"defaultValue: ({typeName})0");
                }
            }
            else
            {
                var formattedValue = TypeFormatting.FormatDefaultValue(param.DefaultValue, param.Type);
                argumentLines.Add($"defaultValue: {formattedValue}");
            }
        }
        else
        {
            argumentLines.Add("defaultValue: null");
        }
        argumentLines.Add("minimum: null"); // No min info in ParameterInfo
        argumentLines.Add("maximum: null"); // No max info in ParameterInfo
        return argumentLines;
    }

    /// <summary>
    /// Formats a parameter name for display.
    /// </summary>
    private static string FormatDisplayName(string parameterName)
    {
        // Simple conversion to title case for now - can be enhanced later
        if (string.IsNullOrEmpty(parameterName))
            return string.Empty;

        return char.ToUpperInvariant(parameterName[0]) +
               (parameterName.Length > 1 ? parameterName.Substring(1) : string.Empty);
    }

    /// <summary>
    /// Gets the parameter type name for code generation.
    /// </summary>
    private static string GetParameterType(string fullTypeName)
    {
        // Extract just the type name, not the fully qualified name
        if (fullTypeName.Contains("."))
        {
            int lastDot = fullTypeName.LastIndexOf('.');
            if (lastDot >= 0 && lastDot < fullTypeName.Length - 1)
            {
                return fullTypeName.Substring(lastDot + 1);
            }
        }

        return fullTypeName;
    }

    /// <summary>
    /// Formats an array of strings for code generation.
    /// </summary>
    private static string[] FormatStringArray(string[] strings)
    {
        var result = new string[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            result[i] = $"\"{strings[i]}\"";
        }
        return result;
    }
}
