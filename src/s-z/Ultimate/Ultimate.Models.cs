namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class UltimateResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Ultimate { get; set; }

    double IReusableResult.Value => Ultimate.Null2NaN();
}
