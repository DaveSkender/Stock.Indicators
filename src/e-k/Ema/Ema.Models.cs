namespace Skender.Stock.Indicators;

public record struct EmaResult(
    DateTime Timestamp,
    double? Ema = default)
    : IReusableResult
{
    readonly double IReusableResult.Value
        => Ema.Null2NaN();
}

public interface IEma : IStreamObserver
{
    int LookbackPeriods { get; }
    double K { get; }
}
