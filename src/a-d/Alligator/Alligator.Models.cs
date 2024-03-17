namespace Skender.Stock.Indicators;

[Serializable]
public sealed record class AlligatorResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}
