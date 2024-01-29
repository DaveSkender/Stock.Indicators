namespace Skender.Stock.Indicators;

public class GatorResult : IResult
{
    public DateTime TickDate { get; set; }
    public double? Upper { get; set; }
    public double? Lower { get; set; }

    public bool? UpperIsExpanding { get; set; }
    public bool? LowerIsExpanding { get; set; }
}
