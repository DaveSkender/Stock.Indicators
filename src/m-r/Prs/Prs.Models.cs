namespace Skender.Stock.Indicators;

[Serializable]
public sealed class PrsResult : ResultBase, IReusableResult
{
    public PrsResult(DateTime date)
    {
        Date = date;
    }

    public double? Prs { get; set; }
    public double? PrsPercent { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? PrsSma { get; set; }

    double IReusableResult.Value => Prs.Null2NaN();
}
