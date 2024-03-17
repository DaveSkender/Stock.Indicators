namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class TrixResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ema3 { get; set; }
    public double? Trix { get; set; }

    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average signal.", false)]
    public double? Signal { get; set; }

    double IReusableResult.Value => Trix.Null2NaN();
}
