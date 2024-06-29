namespace Skender.Stock.Indicators;

public readonly record struct SmaResult
(
    DateTime Timestamp,
    double? Sma = null
) : IReusable
{
    double IReusable.Value => Sma.Null2NaN();
}

/// <summary>
/// SMA with extended analysis.
/// </summary>
/// <param name="Timestamp">Timestamp</param>
/// <param name="Sma">Simple moving average</param>
/// <param name="Mad">Mean absolute deviation</param>
/// <param name="Mse">Mean square error</param>
/// <param name="Mape">Mean absolute percentage error</param>
public readonly record struct SmaAnalysis
(
    DateTime Timestamp,
    double? Sma,
    double? Mad,
    double? Mse,
    double? Mape
) : IReusable
{
    double IReusable.Value => Sma.Null2NaN();
}

public interface ISma : IStreamObserver
{
    int LookbackPeriods { get; }
}
