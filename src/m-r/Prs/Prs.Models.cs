namespace Skender.Stock.Indicators;

[Serializable]
public sealed class PrsResult : ResultBase, IReusableResult
{
    public double? Prs { get; set; }
    public double? PrsSma { get; set; }
    public double? PrsPercent { get; set; }

    double? IReusableResult.Value => Prs;
}
