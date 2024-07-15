namespace Skender.Stock.Indicators;

/// <summary>
/// Quote provider (abstract base)
/// </summary>
public class QuoteHub<TQuote> : QuoteObserver<TQuote, TQuote>,
    IQuoteHub<TQuote, TQuote>
    where TQuote : IQuote
{
    public QuoteHub() : base(new QuoteProvider<TQuote>()) { }

    public QuoteHub(
        IQuoteProvider<TQuote> provider)
        : base(provider)
    {
        Reinitialize();
    }

    public IReadOnlyList<TQuote> Quotes => Cache;

    // METHODS

    public override string ToString()
        => $"QUOTES: {Quotes.Count} items (type: {nameof(TQuote)})";

    public override void Add(TQuote newIn)
    {
        try
        {
            Act act = Modify(newIn);
            NotifyObservers(act, newIn);
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }
}

/// <summary>
/// Empty quote provider for parent-less QuoteHub
/// </summary>
/// <typeparam name="TQuote"></typeparam>
internal class QuoteProvider<TQuote>
    : StreamProvider<TQuote>, IQuoteProvider<TQuote>
    where TQuote : IQuote;
