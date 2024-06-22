namespace Skender.Stock.Indicators;

public record struct AtrResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Tr { get; set; }
    public double? Atr { get; set; }
    public double? Atrp { get; set; }

    readonly double IReusableResult.Value
        => Atrp.Null2NaN();
}
