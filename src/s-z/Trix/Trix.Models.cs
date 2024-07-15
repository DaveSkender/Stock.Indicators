namespace Skender.Stock.Indicators;

public record TrixResult
(
    DateTime Timestamp,
    double? Ema3 = null,
    double? Trix = null
) : Reusable(Timestamp)
{
    public override double Value => Trix.Null2NaN();
}
