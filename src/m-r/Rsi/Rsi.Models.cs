namespace Skender.Stock.Indicators;

public class RsiState
{
    public RsiState(int lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
    }

    public int LookbackPeriods { get; set; }
    public double AvgLoss { get; set; }
    public double AvgGain { get; set; }
}

[Serializable]
public class RsiResult : ResultBase, IReusableResult
{
    public double? Rsi { get; set; }
    public double? Value
    {
        get { return Rsi; }
    }
}
