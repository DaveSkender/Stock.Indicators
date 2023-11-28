namespace Skender.Stock.Indicators;

public interface IChainProvider<TResult> : IProvider<TResult>
    where TResult : IReusableResult, new()
{
    internal new List<TResult> Cache { get; set; }
}

public interface IProvider<TSeries> : IResultCache<TSeries>
    where TSeries : ISeries, new()
{
    // provider contains additional overflow and other common provider (observable) controls

    // PROPERTIES

    public int OverflowCount { get; internal set; }

    public TSeries LastArrival { get; internal set; }
}

public interface IResultCache<TSeries>
    where TSeries : ISeries, new()
{
    // general indicator result cache

    // PROPERTIES

    public IEnumerable<TSeries> Results { get; }

    internal List<TSeries> Cache { get; set; }

    // METHODS

    void Initialize();
}
