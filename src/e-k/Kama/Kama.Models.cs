namespace Skender.Stock.Indicators;

public record KamaResult
(
    DateTime Timestamp,
    double? Er = null,
    double? Kama = null
) : Reusable(Timestamp)
{
    public override double Value => Kama.Null2NaN();
}
