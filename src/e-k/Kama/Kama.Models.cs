namespace Skender.Stock.Indicators;

public record KamaResult
(
    DateTime Timestamp,
    double? Er = null,
    double? Kama = null
) : IReusable
{
    public double Value => Kama.Null2NaN();
}
