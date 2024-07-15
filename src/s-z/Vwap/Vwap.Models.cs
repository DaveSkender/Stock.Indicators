namespace Skender.Stock.Indicators;

public record VwapResult
(
    DateTime Timestamp,
    double? Vwap
) : Reusable(Timestamp)
{
    public override double Value => Vwap.Null2NaN();
}
