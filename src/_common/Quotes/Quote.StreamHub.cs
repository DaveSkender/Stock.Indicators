namespace Skender.Stock.Indicators;

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote, TQuote>
    where TQuote : IQuote
{
    public QuoteHub() : base(new DefaultQuoteProvider<TQuote>()) { }

    public QuoteHub(
        IQuoteProvider<TQuote> provider)
        : base(provider)
    {
        Reinitialize();
    }

    // METHODS

    protected override (TQuote result, int index)
        ToIndicator(TQuote item, int? indexHint)
    {
        int index = indexHint
            ?? Cache.GetIndexGte(item.Timestamp);

        return (item, index == -1 ? Cache.Count : index);
    }

    public override string ToString()
        => $"QUOTES<{typeof(TQuote).Name}>: {Quotes.Count} items";
}

/// <summary>
/// Default quote provider for parent-less QuoteHub
/// </summary>
/// <typeparam name="TQuote"></typeparam>
internal class DefaultQuoteProvider<TQuote>
    : IQuoteProvider<TQuote>
    where TQuote : IQuote
{
    /// <summary>
    /// Default quote provider is parent-less;
    /// but does not transfer that setting to its children.
    /// </summary>
    public BinarySettings Properties { get; } = new(0b00000001, 0b11111110);

    // TODO: are we using this ^^ "parent-less" property at all?
    // Refactor to use it, or remove it.

    public int ObserverCount => 1;
    public bool HasObservers => true;

    public IReadOnlyList<TQuote> Quotes { get; } = Array.Empty<TQuote>();
    public IReadOnlyList<TQuote> GetCacheRef() => Array.Empty<TQuote>();

    public bool HasSubscriber(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public IDisposable Subscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public bool Unsubscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public void EndTransmission()
        => throw new InvalidOperationException();
}
