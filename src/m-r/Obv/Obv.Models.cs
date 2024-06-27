namespace Skender.Stock.Indicators;

public record struct ObvResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double Obv { get; set; }

    readonly double IReusable.Value => Obv;
}
