namespace Skender.Stock.Indicators;

public interface IAlligatorResult
{
    public double? Jaw { get; }
    public double? Teeth { get; }
    public double? Lips { get; }
}

[Serializable]
public sealed class AlligatorResult : ResultBase, IAlligatorResult
{
    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}
