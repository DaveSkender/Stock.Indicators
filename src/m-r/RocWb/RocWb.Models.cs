namespace Skender.Stock.Indicators;

[Serializable]
public record RocWbResult
(
    DateTime Timestamp,
    double? Roc,
    double? RocEma,
    double? UpperBand,
    double? LowerBand
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Roc.Null2NaN();
}
