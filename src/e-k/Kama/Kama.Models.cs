namespace Skender.Stock.Indicators;

[Serializable]
public record KamaResult
(
    DateTime Timestamp,
    double? Er = null,
    double? Kama = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Kama.Null2NaN();
}
