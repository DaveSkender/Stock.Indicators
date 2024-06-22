namespace Skender.Stock.Indicators;

public record struct UltimateResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ultimate { get; set; }

    readonly double IReusableResult.Value
        => Ultimate.Null2NaN();
}
