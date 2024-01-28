namespace Skender.Stock.Indicators;

public sealed class EmaResult : ResultBase, IReusableResult
{
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
