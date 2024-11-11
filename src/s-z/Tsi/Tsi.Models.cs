namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the result of a True Strength Index (TSI) calculation.
/// </summary>
/// <param name="Timestamp">The timestamp of the data point.</param>
/// <param name="Tsi">The value of the TSI at this point.</param>
/// <param name="Signal">The signal line value at this point.</param>
[Serializable]
public record TsiResult
(
    DateTime Timestamp,
    double? Tsi = null,
    double? Signal = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Tsi.Null2NaN();
}
