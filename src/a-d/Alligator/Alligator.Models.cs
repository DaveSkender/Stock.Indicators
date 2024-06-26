namespace Skender.Stock.Indicators;

public record struct AlligatorResult
(
    DateTime Timestamp,
    double? Jaw,
    double? Teeth,
    double? Lips
) : IResult;

public interface IAlligator : IStreamObserver
{
    int JawPeriods { get; }
    int JawOffset { get; }
    int TeethPeriods { get; }
    int TeethOffset { get; }
    int LipsPeriods { get; }
    int LipsOffset { get; }
}
