namespace Skender.Stock.Indicators;

public sealed record class AlligatorResult : IResult
{
    public DateTime TickDate { get; set; }
    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}
