namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HtlResult : ResultBase, IReusableResult
{
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    double? IReusableResult.Value => Trendline;
}
