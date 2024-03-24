namespace Skender.Stock.Indicators;

public sealed record class BasicResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
