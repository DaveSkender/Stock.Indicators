namespace Skender.Stock.Indicators;

[Serializable]
public record TrResult(
    DateTime Timestamp,
    double? Tr
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Tr.Null2NaN();
}
