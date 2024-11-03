namespace Skender.Stock.Indicators;

[Serializable]
public record ObvResult
(
    DateTime Timestamp,
    double Obv
) : IReusable
{
    /// <inheritdoc/>
    public double Value => Obv;
}
