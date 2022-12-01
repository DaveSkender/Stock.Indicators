namespace Skender.Stock.Indicators;

[Serializable]
public class UlcerIndexResult : ResultBase, IReusableResult
{
    public UlcerIndexResult(DateTime date)
    {
        Date = date;
    }

    public double? UI { get; set; } // ulcer index

    double? IReusableResult.Value => UI;
}
