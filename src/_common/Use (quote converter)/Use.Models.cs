namespace Skender.Stock.Indicators;

// TODO: this is redundant to "BasicResult", but it has a funny name
[Serializable]
public sealed class UseResult : ResultBase, IReusableResult
{
    public double Value { get; set; }
}
