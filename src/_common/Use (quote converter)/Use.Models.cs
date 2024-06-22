namespace Skender.Stock.Indicators;

public record struct QuotePart(
    DateTime Timestamp,
    double Value
) : IReusableResult;

public interface IUse : IStreamObserver
{
    CandlePart CandlePartSelection { get; }
}
