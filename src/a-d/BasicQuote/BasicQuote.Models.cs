namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class BasicResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
