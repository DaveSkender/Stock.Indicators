namespace Skender.Stock.Indicators;

[Serializable]
public record ParabolicSarResult
(
    DateTime Timestamp,
    double? Sar = null,
    bool? IsReversal = null
) : IReusable
{
    public double Value => Sar.Null2NaN();
}
