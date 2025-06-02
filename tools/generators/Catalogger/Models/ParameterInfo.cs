// ParameterInfo.cs
// Model for representing parameter information in CatalogGenerator
using Microsoft.CodeAnalysis;

namespace Generators.Catalogger.Models;

/// <summary>
/// Represents information about a parameter extracted from a method
/// </summary>
public class ParameterInfo
{
    /// <summary>
    /// The parameter name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The parameter type
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Whether the parameter has a default value
    /// </summary>
    public bool HasExplicitDefaultValue { get; }

    /// <summary>
    /// The default value for the parameter, if any
    /// </summary>
    public object? ExplicitDefaultValue { get; }

    /// <summary>
    /// The string representation of the default value (for code generation)
    /// </summary>
    public string? DefaultValue { get; }

    /// <summary>
    /// Creates a new instance of ParameterInfo
    /// </summary>
    public ParameterInfo(string name, string type, bool hasExplicitDefaultValue, object? explicitDefaultValue, string? defaultValue)
    {
        Name = name;
        Type = type;
        HasExplicitDefaultValue = hasExplicitDefaultValue;
        ExplicitDefaultValue = explicitDefaultValue;
        DefaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new instance of ParameterInfo from an IParameterSymbol
    /// </summary>
    public static ParameterInfo FromParameterSymbol(IParameterSymbol symbol)
    {
        string? defaultValueString = null;

        if (symbol.HasExplicitDefaultValue)
        {
            // Convert the default value to a string for code generation
            defaultValueString = GetDefaultValueString(symbol.ExplicitDefaultValue, symbol.Type.ToDisplayString());
        }

        return new ParameterInfo(
            symbol.Name,
            symbol.Type.ToDisplayString(),
            symbol.HasExplicitDefaultValue,
            symbol.ExplicitDefaultValue,
            defaultValueString);
    }

    /// <summary>
    /// Extracts a proper string representation of a default value for code generation
    /// </summary>
    private static string? GetDefaultValueString(object? value, string typeName)
    {
        if (value == null)
        {
            return "null";
        }

        // Handle numeric types with invariant culture and correct suffixes
        var type = value.GetType();
        if (type == typeof(double))
            return ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(decimal))
            return ((decimal)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(float))
            return ((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (type == typeof(bool))
            return value.ToString()!.ToLowerInvariant();
        if (type == typeof(string))
            return $"\"{value.ToString()}\"";
        return value.ToString();
    }
}
