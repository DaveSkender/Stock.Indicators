namespace Skender.Stock.Indicators;

public sealed record class AdlResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double Adl { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public double? AdlSma { get; set; }

    double IReusableResult.Value => Adl;
}
