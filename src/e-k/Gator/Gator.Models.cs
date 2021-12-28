namespace Skender.Stock.Indicators;

[Serializable]
public class GatorResult : ResultBase
{
    public double? Upper { get; set; }
    public double? Lower { get; set; }

    public bool? UpperIsExpanding { get; set; }
    public bool? LowerIsExpanding { get; set; }
}
