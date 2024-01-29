namespace Skender.Stock.Indicators;

public sealed record class ObvResult : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double Obv { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public double? ObvSma { get; set; }

    double IReusableResult.Value => Obv;
}
