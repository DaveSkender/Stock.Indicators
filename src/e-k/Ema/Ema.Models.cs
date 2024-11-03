namespace Skender.Stock.Indicators;

[Serializable]
public record EmaResult
(
    DateTime Timestamp,
    double? Ema = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Ema.Null2NaN();
}
