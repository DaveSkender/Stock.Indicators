namespace Skender.Stock.Indicators;

public sealed record class AdxResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Pdi { get; set; }
    public double? Mdi { get; set; }
    public double? Adx { get; set; }
    public double? Adxr { get; set; }

    double IReusableResult.Value => Adx.Null2NaN();
}
