namespace Skender.Stock.Indicators;

[Serializable]
public class BetaResult : ResultBase
{
    public double? Beta { get; set; }
    public double? BetaUp { get; set; }
    public double? BetaDown { get; set; }
    public double? Ratio { get; set; }
    public double? Convexity { get; set; }
}

public enum BetaType
{
    Standard,
    Up,
    Down,
    All
}
