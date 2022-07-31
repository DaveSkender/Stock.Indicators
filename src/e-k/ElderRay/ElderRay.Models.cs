namespace Skender.Stock.Indicators;

[Serializable]
public sealed class ElderRayResult : ResultBase, IReusableResult
{
    public ElderRayResult(DateTime date)
    {
        Date = date;
    }

    public double? Ema { get; set; }
    public double? BullPower { get; set; }
    public double? BearPower { get; set; }

    double? IReusableResult.Value => BullPower + BearPower;
}
