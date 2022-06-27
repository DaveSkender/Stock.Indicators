namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AlligatorResult : ResultBase
{
    public AlligatorResult(DateTime date)
    {
        Date = date;
    }

    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}
