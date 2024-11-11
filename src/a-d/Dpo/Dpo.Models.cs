namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Detrended Price Oscillator (DPO) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="Dpo">The DPO value.</param>
/// <param name="Sma">The Simple Moving Average (SMA) value.</param>
[Serializable]
public record DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : IReusable
{
    /// <inheritdoc/>
    public double Value => Dpo.Null2NaN();
}
