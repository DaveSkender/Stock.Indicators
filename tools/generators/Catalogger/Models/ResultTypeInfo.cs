// ResultTypeInfo.cs
// Model for representing indicator result type information
namespace Generators.Catalogger.Models;

/// <summary>
/// Represents information about the result type of an indicator
/// </summary>
public class ResultTypeInfo
{
    /// <summary>
    /// The result type as a string for code generation
    /// </summary>
    public string ResultType { get; }

    /// <summary>
    /// Creates a new instance of ResultTypeInfo
    /// </summary>
    public ResultTypeInfo(string resultType)
    {
        ResultType = resultType;
    }
}
