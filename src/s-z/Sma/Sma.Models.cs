namespace Skender.Stock.Indicators;

public readonly record struct SmaResult
(
    DateTime Timestamp,
    double? Sma = null
) : IReusable
{
    double IReusable.Value => Sma.Null2NaN();
}

public interface ISma : IStreamObserver
{
    int LookbackPeriods { get; }
}
