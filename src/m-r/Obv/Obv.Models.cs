namespace Skender.Stock.Indicators;

public record ObvResult
(
    DateTime Timestamp,
    double Obv
) : IReusable
{
    public double Value => Obv;
}
