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

    internal override void Add(Act act, TQuote newIn, int? index)
    {
        try
        {
            // save and send
            Motify(act, newIn, index);
        }
        catch (OverflowException)
        {
            EndTransmission();
            throw;
        }
    }

    public override string ToString()
        => $"QUOTES: {Quotes.Count} items (type: {nameof(TQuote)})";
}

/// <summary>
/// Empty quote provider for parent-less QuoteHub
/// </summary>
/// <typeparam name="TQuote"></typeparam>
internal class QuoteProvider<TQuote>
    : StreamProvider<TQuote>, IQuoteProvider<TQuote>
    where TQuote : IQuote;
