namespace Skender.Stock.Indicators;

public record HmaResult
(
    DateTime Timestamp,
    double? Hma = null
) : Reusable(Timestamp)
{
    public override double Value => Hma.Null2NaN();
}
