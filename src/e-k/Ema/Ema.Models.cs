namespace Skender.Stock.Indicators;

public sealed record class EmaResult : IReusableResult
{
    public DateTime TickDate { get; set; }
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
