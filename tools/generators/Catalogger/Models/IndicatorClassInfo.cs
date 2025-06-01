// IndicatorClassInfo.cs
// Model for representing indicator class information
using System.Collections.Generic;

namespace Generators.Catalogger.Models;

/// <summary>
/// Represents information about an indicator class
/// </summary>
public class IndicatorClassInfo
{
    /// <summary>
    /// The name of the class
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// The full display string of the class (with namespace)
    /// </summary>
    public string ClassFullName { get; }

    /// <summary>
    /// The namespace of the class
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// The primary method name for the indicator
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Information extracted from the indicator attribute
    /// </summary>
    public IndicatorAttributeInfo AttributeInfo { get; }

    /// <summary>
    /// The parameters of the indicator method
    /// </summary>
    public IReadOnlyList<ParameterInfo> Parameters { get; }

    /// <summary>
    /// Information about the result type of the indicator
    /// </summary>
    public ResultTypeInfo ResultInfo { get; }

    /// <summary>
    /// Whether the class has an existing Listing property
    /// </summary>
    public bool HasExistingListing { get; }

    /// <summary>
    /// Whether the indicator has multiple styles
    /// </summary>
    public bool HasMultipleStyles { get; }

    /// <summary>
    /// The styles available for this indicator
    /// </summary>
    public string[] Styles { get; }

    /// <summary>
    /// Creates a new instance of IndicatorClassInfo
    /// </summary>
    public IndicatorClassInfo(
        string className,
        string classFullName,
        string @namespace,
        string methodName,
        IndicatorAttributeInfo attributeInfo,
        IReadOnlyList<ParameterInfo> parameters,
        ResultTypeInfo resultInfo,
        bool hasExistingListing,
        bool hasMultipleStyles,
        string[] styles)
    {
        ClassName = className;
        ClassFullName = classFullName;
        Namespace = @namespace;
        MethodName = methodName;
        AttributeInfo = attributeInfo;
        Parameters = parameters;
        ResultInfo = resultInfo;
        HasExistingListing = hasExistingListing;
        HasMultipleStyles = hasMultipleStyles;
        Styles = styles;
    }
}
