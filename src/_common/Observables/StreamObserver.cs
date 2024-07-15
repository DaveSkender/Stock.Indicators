
namespace Skender.Stock.Indicators;

// STREAM OBSERVER (BASE) - observer members only
// with SUBJECT BASE: PROVIDER and CACHE

public abstract class QuoteObserver<TIn, TOut>(
    IQuoteProvider<TIn> provider
) : StreamHub<TIn, TOut>(provider)
    where TIn : IQuote
    where TOut : ISeries;

public abstract class ReusableObserver<TIn, TOut>(
    IChainProvider<TIn> provider
) : StreamHub<TIn, TOut>(provider)
    where TIn : IReusable
    where TOut : ISeries;
