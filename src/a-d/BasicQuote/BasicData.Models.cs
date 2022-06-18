namespace Skender.Stock.Indicators;

public interface IBasicData
{
    public DateTime Date { get; }
    public double Value { get; }
}

public class BasicData : IResult, IBasicData, IReusableResult
{
    public DateTime Date { get; internal set; }
    public double Value { get; internal set; }

    double? IReusableResult.Value => Value;
}
