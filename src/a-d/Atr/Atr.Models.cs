namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class AtrResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tr { get; set; }
    public double? Atr { get; set; }
    public double? Atrp { get; set; }

    double IReusableResult.Value => Atrp.Null2NaN();
}
