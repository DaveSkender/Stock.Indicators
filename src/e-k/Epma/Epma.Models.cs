namespace Skender.Stock.Indicators;

[Serializable]
public record EpmaResult
(
    DateTime Timestamp,
    double? Epma
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Epma.Null2NaN();
}
