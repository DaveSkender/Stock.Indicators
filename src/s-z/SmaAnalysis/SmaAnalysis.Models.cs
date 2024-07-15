namespace Skender.Stock.Indicators;

/// <summary>
/// SMA with extended analysis.
/// </summary>
/// <param name="Timestamp">Timestamp</param>
/// <param name="Sma">Simple moving average</param>
/// <param name="Mad">Mean absolute deviation</param>
/// <param name="Mse">Mean square error</param>
/// <param name="Mape">Mean absolute percentage error</param>
public record SmaAnalysis
(
    DateTime Timestamp,
    double? Sma = null,
    double? Mad = null,
    double? Mse = null,
    double? Mape = null
) : SmaResult(Timestamp, Sma);
