namespace Skender.Stock.Indicators;

[Serializable]
public record SmiResult
(
    DateTime Timestamp,
    double? Smi,
    double? Signal
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Smi.Null2NaN();
}
