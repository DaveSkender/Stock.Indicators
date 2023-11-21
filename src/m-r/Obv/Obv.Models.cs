namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ObvResult : ResultBase, IReusableResult
{
    public ObvResult(DateTime date)
    {
        Date = date;
    }

    public double Obv { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public double? ObvSma { get; set; }

    double? IReusableResult.Value => Obv;
}
