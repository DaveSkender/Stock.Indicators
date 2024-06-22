namespace Skender.Stock.Indicators;

public record struct PrsResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Prs { get; set; }
    public double? PrsPercent { get; set; }

    readonly double IReusableResult.Value
        => Prs.Null2NaN();
}
