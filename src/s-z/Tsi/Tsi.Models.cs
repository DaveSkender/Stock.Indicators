namespace Skender.Stock.Indicators;

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
