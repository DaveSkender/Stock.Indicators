namespace Skender.Stock.Indicators;

public readonly record struct EmaResult
(
    DateTime Timestamp,
    double? Ema = null
) : IReusable
{
    double IReusable.Value => Ema.Null2NaN();
}

public interface IEma : IStreamObserver
{
    int LookbackPeriods { get; }
    double K { get; }
}
