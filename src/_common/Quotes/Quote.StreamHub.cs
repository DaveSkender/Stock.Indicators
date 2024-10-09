namespace Skender.Stock.Indicators;

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public class QuoteHub<TQuote>
    : QuoteProvider<TQuote, TQuote>
    where TQuote : IQuote
{
    public QuoteHub() : base(new EmptyQuoteProvider<TQuote>()) { }

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
/// Empty quote provider for base Quote Hub initialization.
/// </summary>
/// <remarks>Internal use only. Do not use directly.</remarks>
/// <typeparam name="TQuote"></typeparam>
public class EmptyQuoteProvider<TQuote>
    : IQuoteProvider<TQuote>
    where TQuote : IQuote
{
    /// <summary>
    /// Default quote provider is parent-less Quote Hub.
    /// It does not transfer its setting to its children.
    /// </summary>
    public BinarySettings Properties { get; } = new(0b00000001, 0b11111110);
    public int ObserverCount => 0;
    public bool HasObservers => false;
    public IReadOnlyList<TQuote> Quotes { get; } = Array.Empty<TQuote>();
    public IReadOnlyList<TQuote> GetCacheRef() => Array.Empty<TQuote>();
    public bool HasSubscriber(IStreamObserver<TQuote> observer) => false;

    public IDisposable Subscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public bool Unsubscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public void EndTransmission()
        => throw new InvalidOperationException();
}
