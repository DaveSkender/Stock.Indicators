namespace Skender.Stock.Indicators;

public interface IBaseQuote
{
    public DateTime Date { get; }
    public double Value { get; }
}

public class BaseQuote : IBaseQuote
{
    public DateTime Date { get; internal set; }
    public double Value { get; internal set; }
}