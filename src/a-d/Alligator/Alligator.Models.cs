namespace Skender.Stock.Indicators;

[Serializable]
public sealed class AlligatorResult : ResultBase
{
    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}
