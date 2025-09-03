namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Simple Moving Average (SMA) calculation
/// with extended analysis values.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Sma">The Simple Moving Average (SMA) at this point.</param>
/// <param name="Mad">The Mean Absolute Deviation (MAD) at this point.</param>
/// <param name="Mse">The Mean Square Error (MSE) at this point.</param>
/// <param name="Mape">The Mean Absolute Percentage Error (MAPE) at this point.</param>
[Serializable]
public record SmaAnalysisResult
(
    DateTime Timestamp,
    double? Sma = null,
    double? Mad = null,
    double? Mse = null,
    double? Mape = null
) : SmaResult(Timestamp, Sma);
