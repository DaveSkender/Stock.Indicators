namespace Skender.Stock.Indicators;

public class GatorResult : ResultBase
{
    public GatorResult(DateTime date)
    {
        Date = date;
    }

    public double? Upper { get; set; }
    public double? Lower { get; set; }

    public bool? UpperIsExpanding { get; set; }
    public bool? LowerIsExpanding { get; set; }
}
