using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// TODO: is this serializable attribute even needed?
// TODO: this "BasicResult" name smells funny, especially if it's showing in stream modifiers
[Serializable]
public sealed class BasicResult : ResultBase, IReusableResult
{
    public double Value { get; set; }
}
