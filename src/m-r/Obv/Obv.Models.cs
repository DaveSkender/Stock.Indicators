namespace Skender.Stock.Indicators;

public record struct ObvResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Obv { get; set; }

    readonly double IReusableResult.Value => Obv;
}
