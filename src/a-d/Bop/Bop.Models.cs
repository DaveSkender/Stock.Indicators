namespace Skender.Stock.Indicators;

[Serializable]
public sealed class BopResult : ResultBase, IReusableResult
{
    public double? Bop { get; set; }

    double? IReusableResult.Value => Bop;
}
