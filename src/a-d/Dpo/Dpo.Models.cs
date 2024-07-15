namespace Skender.Stock.Indicators;

public record DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : Reusable(Timestamp)
{
    public override double Value => Dpo.Null2NaN();
}
