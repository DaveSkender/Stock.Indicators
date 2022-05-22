namespace Skender.Stock.Indicators;

public interface IAdxResult
{
    public double? Pdi { get; }
    public double? Mdi { get; }
    public double? Adx { get; }
}

[Serializable]
public sealed class AdxResult : ResultBase, IAdxResult, IReusableResult
{
    public double? Pdi { get; set; }
    public double? Mdi { get; set; }
    public double? Adx { get; set; }
    public double? Adxr { get; set; }

    double? IReusableResult.Value => Adx;
}
