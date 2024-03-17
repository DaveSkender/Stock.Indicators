namespace Skender.Stock.Indicators;

// TODO: this is redundant to "BasicResult", but it has a funny name
[Serializable]
public sealed record class UseResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
