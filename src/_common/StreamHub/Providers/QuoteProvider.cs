namespace Skender.Stock.Indicators;

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
public abstract class QuoteProvider<TIn, TOut> : StreamHub<TIn, TOut>, IQuoteProvider<TOut>
     where TIn : IReusable
     where TOut : IQuote
{
    /// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
    /// <param name="provider">Streaming data provider.</param>
    internal QuoteProvider(
        IStreamObservable<TIn> provider
    ) : base(provider)
    => Quotes = Cache.AsReadOnly();  // instantiate once

    /// <summary>
    /// Gets the quotes as a read-only collection (safe from external mutation).
    /// </summary>
    public IReadOnlyList<TOut> Quotes { get; }
}
