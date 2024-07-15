namespace Skender.Stock.Indicators;

public record ParabolicSarResult
(
    DateTime Timestamp,
    double? Sar = null,
    bool? IsReversal = null
) : Reusable(Timestamp)
{
    public override double Value => Sar.Null2NaN();
}
