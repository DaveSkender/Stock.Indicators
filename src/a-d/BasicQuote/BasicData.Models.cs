namespace Skender.Stock.Indicators;

public interface IBasicData
{
    public DateTime Date { get; }
    public double Value { get; }
}

public class BasicData : ISeries, IBasicData, IReusableResult
{
    public DateTime Date { get; set; }
    public double Value { get; set; }

    double? IReusableResult.Value => Value;
}
