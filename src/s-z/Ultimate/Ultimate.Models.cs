namespace Skender.Stock.Indicators;

public record struct UltimateResult : IReusable
{
    public DateTime Timestamp { get; set; }
    public double? Ultimate { get; set; }

    readonly double IReusable.Value
        => Ultimate.Null2NaN();
}
