namespace Skender.Stock.Indicators;

public abstract class QuoteObserver<TQuote, TResult> : SeriesCache<TResult>,
    IObserver<(Act act, TQuote quote)>
    where TQuote : IQuote, new()
    where TResult : IResult, new()
{
    internal IDisposable? unsubscriber;

    internal QuoteObserver(QuoteProvider<TQuote> provider)
    {
        QuoteSupplier = provider;
    }

    // properites

    internal QuoteProvider<TQuote> QuoteSupplier { get; }

    // methods

    public abstract void OnNext((Act act, TQuote quote) value);

    public void OnError(Exception error) => throw error;

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    public abstract void Initialize();

    internal abstract void ResetCache();
}
