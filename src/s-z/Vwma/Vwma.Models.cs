namespace Skender.Stock.Indicators;

public record VwmaResult
(
    DateTime Timestamp,
    double? Vwma
) : Reusable(Timestamp)
{
    public override double Value => Vwma.Null2NaN();
}
