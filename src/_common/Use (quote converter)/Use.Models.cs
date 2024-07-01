namespace Skender.Stock.Indicators;

// note: Use() uses the Reusable result record type

public interface IUse<TQuote> : IQuoteObserver<TQuote>
    where TQuote : struct, IQuote
{
    CandlePart CandlePartSelection { get; }
}
