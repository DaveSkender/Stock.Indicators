namespace Skender.Stock.Indicators;

// TODO: is this serializable attribute even needed?
// TODO: this "BasicData" name smells funny, especially if it's showing in stream modifiers
[Serializable]
public sealed class BasicData : ResultBase, IReusableResult
{
    public double Value { get; set; }
}
