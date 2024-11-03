namespace Skender.Stock.Indicators;

[Serializable]
public record RsiResult
(
    DateTime Timestamp,
    double? Rsi = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Rsi.Null2NaN();
}
