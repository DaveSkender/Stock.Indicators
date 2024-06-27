namespace Skender.Stock.Indicators;

public record struct EmaResult(
    DateTime Timestamp,
    double? Ema = null)
    : IReusable
{
    readonly double IReusable.Value
        => Ema.Null2NaN();
}

public interface IEma : IStreamObserver
{
    int LookbackPeriods { get; }
    double K { get; }
}
