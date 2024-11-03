namespace Skender.Stock.Indicators;

[Serializable]
public record WmaResult
(
    DateTime Timestamp,
    double? Wma
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Wma.Null2NaN();
}
