namespace Skender.Stock.Indicators;

// TODO: rename to IChainObservable

/// <inheritdoc/>
public interface IChainProvider<out T> : IStreamObservable<T>
   where T : IReusable;
