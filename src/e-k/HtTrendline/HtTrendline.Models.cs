namespace Skender.Stock.Indicators;

[Serializable]
public sealed class HtlResult : ResultBase, IReusableResult
{
    public HtlResult(DateTime date)
    {
        Date = date;
    }

    public int? DcPeriods { get; set; }
    public double? Trendline { get; set; }
    public double? SmoothPrice { get; set; }

    double? IReusableResult.Value => Trendline;
}
