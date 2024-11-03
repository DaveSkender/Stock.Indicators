namespace Skender.Stock.Indicators;

[Serializable]
public record MamaResult
(
    DateTime Timestamp,
    double? Mama = null,
    double? Fama = null
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Mama.Null2NaN();
}
