namespace Skender.Stock.Indicators;

public record EmaResult
(
    DateTime Timestamp,
    double? Ema = null
) : Reusable(Timestamp)
{
    public override double Value => Ema.Null2NaN();
}
