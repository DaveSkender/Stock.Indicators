namespace Skender.Stock.Indicators;

[Serializable]
public sealed class NewHighsNewLowsResult : ResultBase, IReusableResult
{
    internal NewHighsNewLowsResult(DateTime date)
    {
        Date = date;
    }

    public bool? NewHigh { get; set; }
    public bool? NewLow { get; set; }

    public double? NewHighs { get; set; }
    public double? NewLows { get; set; }

    public DateTime? LastNewHigh { get; set; }
    public DateTime? LastNewLow { get; set; }

    public double? Net { get; set; }
    public double? Cumulative { get; set; }

    public double? RecordHighPercent { get; set; }

    double? IReusableResult.Value => Net;
}
