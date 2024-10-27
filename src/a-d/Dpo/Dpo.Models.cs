namespace Skender.Stock.Indicators;

public record DpoResult
(
    DateTime Timestamp,
    double? Dpo = null,
    double? Sma = null
    ) : IReusable
{
    public double Value => Dpo.Null2NaN();
}
