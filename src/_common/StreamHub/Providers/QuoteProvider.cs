namespace Skender.Stock.Indicators;

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
/// <param name="provider">Streaming data provider.</param>
public abstract class QuoteProvider<TIn, TOut>(
    IStreamObservable<TIn> provider
) : StreamHub<TIn, TOut>(provider), IQuoteProvider<TOut>
     where TIn : IReusable
     where TOut : IQuote
{
    /// <summary>
    /// Gets the quotes as a read-only wrapper.
    /// </summary>
    public IReadOnlyList<TOut> Quotes => Cache.AsReadOnly();
}
