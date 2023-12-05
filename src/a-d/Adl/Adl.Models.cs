namespace Skender.Stock.Indicators;

public sealed class AdlResult : ResultBase, IReusableResult
{
    public AdlResult(DateTime date)
    {
        Date = date;
    }

    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double Adl { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public double? AdlSma { get; set; }

    double IReusableResult.Value => Adl;
}
