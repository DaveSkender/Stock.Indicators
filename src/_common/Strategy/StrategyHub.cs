namespace Skender.Stock.Indicators;

/// <summary>
/// Lightweight container to coordinate streaming hubs when composing strategies.
/// </summary>
/// <typeparam name="TIn">Type of the provider source.</typeparam>
public sealed class StrategyHub<TIn>
    where TIn : IReusable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyHub{TProvider}"/> class.
    /// </summary>
    /// <param name="provider">Base streaming provider used by the strategy.</param>
    public StrategyHub(IChainProvider<TIn> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        Provider = provider;
    }

    /// <summary>
    /// Gets the base provider for the strategy.
    /// </summary>
    public IChainProvider<TIn> Provider { get; }

    /// <summary>
    /// Tracks a hub as part of the strategy and returns the same instance for chaining.
    /// </summary>
    /// <typeparam name="THub">Type of the hub.</typeparam>
    /// <typeparam name="TResult">Result type emitted by the hub.</typeparam>
    /// <param name="hub">Hub to register.</param>
    /// <returns>Same hub instance for fluent usage.</returns>
    public THub Use<THub, TResult>(THub hub)
        where THub : IStreamObservable<TResult>
        where TResult : ISeries
    {
        ArgumentNullException.ThrowIfNull(hub);
        return hub;
    }

    /// <summary>
    /// Attempts to retrieve the most recent and prior results for two hubs at the latest aligned timestamp.
    /// </summary>
    /// <typeparam name="THub1">Result type for hub1.</typeparam>
    /// <typeparam name="THub2">Result type for hub2.</typeparam>
    /// <param name="hub1">First hub to evaluate.</param>
    /// <param name="hub2">Second hub to evaluate.</param>
    /// <param name="hub1Results">Previous and current results for hub1.</param>
    /// <param name="hub2Results">Previous and current results for hub2.</param>
    /// <returns>True when both hubs have at least two results and the most recent timestamps align.</returns>
    public bool TryGetLatest<THub1, THub2>(
        IStreamObservable<THub1> hub1,
        IStreamObservable<THub2> hub2,
        out (THub1 previous, THub1 current) hub1Results,
        out (THub2 previous, THub2 current) hub2Results)
        where THub1 : ISeries
        where THub2 : ISeries
    {
        ArgumentNullException.ThrowIfNull(hub1);
        ArgumentNullException.ThrowIfNull(hub2);

        if (!TryGetLatestPair(hub1, out hub1Results)
            || !TryGetLatestPair(hub2, out hub2Results))
        {
            hub1Results = default!;
            hub2Results = default!;
            return false;
        }

        return hub1Results.current.Timestamp == hub2Results.current.Timestamp;
    }

    private static bool TryGetLatestPair<TResult>(
        IStreamObservable<TResult> hub,
        out (TResult previous, TResult current) pair)
        where TResult : ISeries
    {
        IReadOnlyList<TResult> cache = hub.Results;

        if (cache.Count < 2)
        {
            pair = default!;
            return false;
        }

        pair = (cache[^2], cache[^1]);
        return true;
    }
}
