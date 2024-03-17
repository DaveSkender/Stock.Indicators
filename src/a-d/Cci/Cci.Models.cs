namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class CciResult : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double? Cci { get; set; }

    double IReusableResult.Value => Cci.Null2NaN();
}
