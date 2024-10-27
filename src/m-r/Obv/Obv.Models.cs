namespace Skender.Stock.Indicators;

[Serializable]
public record ObvResult
(
    DateTime Timestamp,
    double Obv
) : IReusable
{
    public double Value => Obv;
}
