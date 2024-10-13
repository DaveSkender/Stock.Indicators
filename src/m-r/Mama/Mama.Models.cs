namespace Skender.Stock.Indicators;

public record MamaResult
(
    DateTime Timestamp,
    double? Mama = null,
    double? Fama = null
) : IReusable
{
    public double Value => Mama.Null2NaN();
}
