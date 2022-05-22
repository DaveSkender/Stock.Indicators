namespace Skender.Stock.Indicators;

public interface IStarcBandsResult
{
    public double? UpperBand { get; }
    public double? Centerline { get; }
    public double? LowerBand { get; }
}

[Serializable]
public sealed class StarcBandsResult : ResultBase, IStarcBandsResult
{
    public double? UpperBand { get; set; }
    public double? Centerline { get; set; }
    public double? LowerBand { get; set; }
}
