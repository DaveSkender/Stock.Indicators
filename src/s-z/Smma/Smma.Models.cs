namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class SmmaResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Smma { get; set; }

    double IReusableResult.Value => Smma.Null2NaN();
}
