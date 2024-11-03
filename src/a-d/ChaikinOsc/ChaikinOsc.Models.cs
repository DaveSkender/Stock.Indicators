namespace Skender.Stock.Indicators;

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
