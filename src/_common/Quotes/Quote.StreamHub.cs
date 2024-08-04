namespace Skender.Stock.Indicators;

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote, TQuote>
    where TQuote : IQuote
{
    public QuoteHub() : base(new QuoteProvider<TQuote>()) { }

    public QuoteHub(
        IQuoteProvider<TQuote> provider)
        : base(provider)
    {
        Reinitialize();
    }

    // METHODS

    protected override (TQuote result, int? index)
        ToCandidate(TQuote item, int? indexHint)
    {
        int? index = indexHint
            ?? Cache.GetIndexGte(item.Timestamp);

        return (item, index == -1 ? null : index);
    }

    public override string ToString()
        => $"QUOTES<{typeof(TQuote).Name}>: {Quotes.Count} items";
}

/// <summary>
/// Empty quote provider for parent-less QuoteHub
/// </summary>
/// <typeparam name="TQuote"></typeparam>
internal class QuoteProvider<TQuote>
    : IQuoteProvider<TQuote>
    where TQuote : IQuote
{
    // TODO: this doesn't smell right, but the only other
    // option is to make Provider nullable in StreamHub;
    // which will require significant defensive coding elsewhere.

    // TODO: QuoteProvider<T> name may be confused with the StreamHub variant

    public bool HasObservers => true;
    public int ObserverCount => 1;

    public IReadOnlyList<TQuote> GetCacheRef() => [];

    public IReadOnlyList<TQuote> Quotes { get; } = [];

    public bool HasSubscriber(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public IDisposable Subscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public bool Unsubscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public void EndTransmission()
        => throw new InvalidOperationException();

}
