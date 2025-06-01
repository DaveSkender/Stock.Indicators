// TypeFormatting.cs
// Helper methods for formatting type values in the CatalogGenerator
using System;
using System.Globalization;

namespace Generators.Catalogger.Helpers;

/// <summary>
/// Helper methods for formatting type values for code generation
/// </summary>
public static class TypeFormatting
{
    /// <summary>
    /// Formats a default value for code generation based on its type.
    /// </summary>
    /// <param name="defaultValue">The default value as a string</param>
    /// <param name="type">The type of the value</param>
    /// <returns>Properly formatted string for code generation</returns>
    public static string FormatDefaultValue(string? defaultValue, string type)
    {
        if (string.IsNullOrEmpty(defaultValue) || defaultValue == "null")
            return "null";

        // Check if it's an enum type
        if (IsEnumType(type))
        {
            return FormatEnumValue(type, defaultValue);
        }

        // For single character inputs that might be type suffixes
        if (defaultValue?.Length == 1 && char.TryParse(defaultValue, out char c))
        {
            return HandleSpecialCharacter(c);
        }

        try
        {
            // Clean value by removing any type suffixes
            var cleanValue = defaultValue!.Trim();
            char lastChar = cleanValue.Length > 0 ? cleanValue[cleanValue.Length - 1] : '\0';

            // Remove type suffix if present
            if (lastChar == 'f' || lastChar == 'F' || lastChar == 'm' || lastChar == 'M' ||
                lastChar == 'd' || lastChar == 'D' || lastChar == 'l' || lastChar == 'L')
            {
                cleanValue = cleanValue.Substring(0, cleanValue.Length - 1);
            }

            // Try to parse the numeric value
            if (!double.TryParse(cleanValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var numericValue))
            {
                // If parsing fails, format based on type
                var result = type.ToLowerInvariant() switch
                {
                    "string" or "system.string" => $"\"{defaultValue}\"",
                    "bool" or "system.boolean" => defaultValue!.ToLowerInvariant(),
                    _ => defaultValue! // For any other type, just return as-is
                };

                return result;
            }

            // Format based on the type
            string typeLower = type.ToLowerInvariant();
            string formattedValue;

            if (typeLower.Contains("float") || typeLower.Contains("single"))
            {
                formattedValue = FormatAsFloat(numericValue);
            }
            else if (typeLower.Contains("decimal"))
            {
                formattedValue = FormatAsDecimal(numericValue);
            }
            else if (typeLower.Contains("double"))
            {
                formattedValue = FormatAsDouble(numericValue);
            }
            else if (typeLower.Contains("long") || typeLower.Contains("int64"))
            {
                formattedValue = numericValue.ToString(CultureInfo.InvariantCulture) + "L";
            }
            else
            {
                // For other numeric types, use a reasonable default format
                formattedValue = numericValue.ToString(CultureInfo.InvariantCulture);
            }

            return formattedValue;
        }
        catch (Exception)
        {
            // Safe default values by type
            return type.ToLowerInvariant() switch
            {
                "string" or "system.string" => "\"\"",
                "bool" or "system.boolean" => "false",
                "double" or "system.double" => "0.0",
                "decimal" or "system.decimal" => "0.0m",
                "float" or "system.single" => "0.0f",
                "int" or "system.int32" => "0",
                "long" or "system.int64" => "0L",
                _ => "null"
            };
        }
    }

    /// <summary>
    /// Formats enum values properly to avoid type duplication issues.
    /// </summary>
    /// <param name="type">The enum type name</param>
    /// <param name="value">The enum value</param>
    /// <returns>Properly formatted enum value reference</returns>
    public static string FormatEnumValue(string type, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return $"({type})0";

        // First, extract the type name without namespace
        string typeName = type;
        if (typeName.Contains("."))
        {
            typeName = typeName.Substring(typeName.LastIndexOf('.') + 1);
        }

        // Try to parse as an integer value first
        if (int.TryParse(value, out var enumValue))
        {
            return $"({typeName}){enumValue}";
        }

        string memberName = value!;
        if (value!.Contains("."))
        {
            string[] parts = value.Split('.');
            memberName = parts[parts.Length - 1]; // Last part is member name
        }

        // Handle cases where the type name equals the member name
        if (typeName == memberName)
        {
            // Most likely we have a Type.Type situation (like "BetaType.BetaType")
            // which is probably an error. Use a safe cast to 0 instead.
            return $"({typeName})0";
        }

        // Check for types embedded in the member name
        if (memberName.StartsWith(typeName) && memberName != typeName)
        {
            // Remove the type prefix from the member name
            memberName = memberName.Substring(typeName.Length);
            if (memberName.StartsWith("."))
            {
                memberName = memberName.Substring(1);
            }
        }

        if (string.IsNullOrEmpty(memberName))
        {
            return $"({typeName})0";
        }

        // Return a properly formatted enum reference
        return $"{typeName}.{memberName}";
    }

    /// <summary>
    /// Handle special single-character type markers.
    /// </summary>
    /// <param name="c">The character to handle</param>
    /// <returns>Properly formatted value based on type marker</returns>
    public static string HandleSpecialCharacter(char c)
    {
        return c switch
        {
            'F' or 'f' => "0.0f", // Float
            'M' or 'm' => "0.0m", // Decimal
            'D' or 'd' => "0.0",  // Double
            'L' or 'l' => "0L",   // Long
            _ => c.ToString()     // Other characters
        };
    }

    /// <summary>
    /// Format a float value properly for code generation.
    /// </summary>
    public static string FormatAsFloat(double value)
    {
        // Special cases for float values
        if (double.IsNaN(value)) return "float.NaN";
        if (double.IsPositiveInfinity(value)) return "float.PositiveInfinity";
        if (double.IsNegativeInfinity(value)) return "float.NegativeInfinity";

        try
        {
            // Simple case: convert to string with invariant culture
            string result = value.ToString(CultureInfo.InvariantCulture);

            // Handle scientific notation by converting to fixed format
            if (result.Contains('e') || result.Contains('E'))
            {
                result = value.ToString("F10", CultureInfo.InvariantCulture)
                             .TrimEnd('0')
                             .TrimEnd('.');
            }

            // Ensure there's a decimal point
            if (!result.Contains('.'))
            {
                result += ".0";
            }

            // Add float suffix
            return result + "f";
        }
        catch
        {
            return "0.0f"; // Safe default for float
        }
    }

    /// <summary>
    /// Format a decimal value properly for code generation.
    /// </summary>
    public static string FormatAsDecimal(double value)
    {
        // Decimal doesn't support special values
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return "0.0m";
        }

        try
        {
            decimal decimalValue;
            try
            {
                // Try to convert to decimal directly
                decimalValue = Convert.ToDecimal(value);
            }
            catch
            {
                // If out of decimal range, use a safe default
                return "0.0m";
            }

            // Format with invariant culture
            string result = decimalValue.ToString(CultureInfo.InvariantCulture);

            // Ensure there's a decimal point
            if (!result.Contains('.'))
            {
                result += ".0";
            }

            // Add decimal suffix
            return result + "m";
        }
        catch
        {
            return "0.0m"; // Safe default for decimal
        }
    }

    /// <summary>
    /// Format a double value properly for code generation.
    /// </summary>
    public static string FormatAsDouble(double value)
    {
        // Special cases for double values
        if (double.IsNaN(value)) return "double.NaN";
        if (double.IsPositiveInfinity(value)) return "double.PositiveInfinity";
        if (double.IsNegativeInfinity(value)) return "double.NegativeInfinity";

        try
        {
            // Format with invariant culture
            string result = value.ToString(CultureInfo.InvariantCulture);

            // Handle scientific notation
            if (result.Contains('e') || result.Contains('E'))
            {
                result = value.ToString("F10", CultureInfo.InvariantCulture)
                             .TrimEnd('0')
                             .TrimEnd('.');
            }

            // Ensure there's a decimal point
            if (!result.Contains('.'))
            {
                result += ".0";
            }

            // No suffix needed for double
            return result;
        }
        catch
        {
            return "0.0"; // Safe default for double
        }
    }

    /// <summary>
    /// Determine if a type is an enum type based on its name.
    /// </summary>
    /// <param name="typeName">The name of the type to check</param>
    /// <returns>True if the type is likely an enum, false otherwise</returns>
    public static bool IsEnumType(string typeName)
    {
        // Common enum types in the library
        string[] knownEnums = new[]
        {
            "AggressionLevel",
            "BetaType",
            "ChandelierExitType",
            "CoppockCurveType",
            "EndType",
            "LookbackPeriod",
            "MaType",
            "PeriodSize",
            "Series"
        };

        foreach (var enumName in knownEnums)
        {
            // Check if type contains enum name (with namespace or not)
            if (typeName.Contains(enumName))
                return true;
        }

        return false;
    }
}
