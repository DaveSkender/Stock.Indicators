namespace Skender.Stock.Indicators;

/// <inheritdoc/>
public interface IChainProvider<out T> : IStreamObservable<T>
   where T : IReusable;
