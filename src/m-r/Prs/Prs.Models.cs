namespace Skender.Stock.Indicators;

public record struct PrsResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Prs { get; set; }
    public double? PrsPercent { get; set; }

    readonly double IReusable.Value
        => Prs.Null2NaN();
}
