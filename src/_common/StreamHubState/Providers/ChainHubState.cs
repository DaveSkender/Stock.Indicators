namespace Skender.Stock.Indicators;

/// <inheritdoc cref="IStreamHub{TIn, TOut}"/>
public abstract class ChainHubState<TIn, TState, TOut>(
    IStreamObservable<TIn> provider
) : StreamHubState<TIn, TState, TOut>(provider), IChainProvider<TOut>
     where TIn : IReusable
     where TOut : IReusable
     where TState : IHubState;
