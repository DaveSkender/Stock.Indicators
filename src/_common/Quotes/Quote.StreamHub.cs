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

    protected override void Add(TQuote item, int? indexHint)
        => Motify(item, indexHint);

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

    public bool HasSubscribers => true;
    public int SubscriberCount => 1;

    public IReadOnlyList<TQuote> Quotes { get; } = [];

    public bool HasSubscriber(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public IDisposable Subscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public void Unsubscribe(IStreamObserver<TQuote> observer)
        => throw new InvalidOperationException();

    public void EndTransmission()
        => throw new InvalidOperationException();

    public IReadOnlyList<TQuote> GetReadOnlyCache()
        => throw new InvalidOperationException();
}
