namespace Skender.Stock.Indicators;

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
public abstract class ChainProvider<TIn, TOut>(
    IStreamObservable<TIn> provider
) : StreamHub<TIn, TOut>(provider), IChainProvider<TOut>  // likely has to be concrete type
     where TIn : IReusable
     where TOut : IReusable;
