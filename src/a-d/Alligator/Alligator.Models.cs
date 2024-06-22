namespace Skender.Stock.Indicators;

public record struct AlligatorResult : IResult
{
    public DateTime Timestamp { get; set; }
    public double? Jaw { get; set; }
    public double? Teeth { get; set; }
    public double? Lips { get; set; }
}

public interface IAlligator : IStreamObserver
{
    int JawPeriods { get; }
    int JawOffset { get; }
    int TeethPeriods { get; }
    int TeethOffset { get; }
    int LipsPeriods { get; }
    int LipsOffset { get; }
}
