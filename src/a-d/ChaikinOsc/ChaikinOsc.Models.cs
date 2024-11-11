namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of the Chaikin Oscillator calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the result.</param>
/// <param name="MoneyFlowMultiplier">The money flow multiplier value.</param>
/// <param name="MoneyFlowVolume">The money flow volume value.</param>
/// <param name="Adl">The Accumulation/Distribution Line (ADL) value.</param>
/// <param name="Oscillator">The Chaikin Oscillator value.</param>
[Serializable]
public record ChaikinOscResult
(
    DateTime Timestamp,
    double? MoneyFlowMultiplier,
    double? MoneyFlowVolume,
    double? Adl,
    double? Oscillator
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Oscillator.Null2NaN();
}
