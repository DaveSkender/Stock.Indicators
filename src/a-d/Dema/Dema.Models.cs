namespace Skender.Stock.Indicators;

public record DemaResult
(
    DateTime Timestamp,
    double? Dema = null
) : Reusable(Timestamp)
{
    public override double Value => Dema.Null2NaN();
}
