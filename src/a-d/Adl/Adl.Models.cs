namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class AdlResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? MoneyFlowMultiplier { get; set; }
    public double? MoneyFlowVolume { get; set; }
    public double Adl { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public double? AdlSma { get; set; }

    double IReusableResult.Value => Adl;
}
