namespace Skender.Stock.Indicators;

[Serializable]
public sealed class StcResult : ResultBase, IReusableResult
{
    public double? Stc { get; set; }

    double? IReusableResult.Value => Stc;
}
