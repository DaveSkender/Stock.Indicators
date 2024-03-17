namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class EmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ema { get; internal set; }

    double IReusableResult.Value => Ema.Null2NaN();
}

public interface IEma :
    IChainObserver<EmaResult>,
    IChainProvider
{
    int LookbackPeriods { get; }
    double K { get; }
}
