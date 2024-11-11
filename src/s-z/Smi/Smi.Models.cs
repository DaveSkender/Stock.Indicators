namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a Stochastic Momentum Index (SMI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Smi">The value of the SMI at this point.</param>
/// <param name="Signal">The signal line value at this point.</param>
[Serializable]
public record SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Smi.Null2NaN();
}
