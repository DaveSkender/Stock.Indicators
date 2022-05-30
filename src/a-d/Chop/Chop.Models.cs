namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ChopResult : ResultBase, IReusableResult
{
    public double? Chop { get; set; }

    double? IReusableResult.Value => Chop;
}
